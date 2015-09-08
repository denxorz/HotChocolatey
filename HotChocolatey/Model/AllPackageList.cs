using NuGet;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotChocolatey.Model
{
    public class AllPackageList : PackageListBase
    {
        private readonly ChocolateyController controller;

        private IOrderedQueryable<IPackage> query;
        private int total;

        public override bool HasMore => total > skipped;

        public AllPackageList(ChocolateyController controller)
        {
            this.controller = controller;
        }

        public override async Task Refresh()
        {
            skipped = 0;
            await Task.Run(() =>
            {
                var baseQuery = GetBaseQuery();
                var includeSearch = string.IsNullOrWhiteSpace(searchFor) ? baseQuery : baseQuery.Where(t => t.Tags.ToLower().Contains(searchFor) || t.Title.ToLower().Contains(searchFor));
                query = includeSearch.OrderByDescending(p => p.DownloadCount);
            }).ContinueWith(task => total = query.Count());
        }

        public override async Task<IEnumerable<ChocoItem>> GetMore(int numberOfItems)
        {
            var tmp = query.Skip(skipped).Take(numberOfItems).ToList();
            skipped += numberOfItems;

            var packages = tmp.Select(t => new ChocoItem(t)).ToList();
            await Task.WhenAll(packages.Select(controller.UpdatePackageVersion));
            packages.ForEach(t => t.Actions = ActionFactory.Generate(controller, t));

            return packages;
        }

        public override async Task ApplySearch(string searchFor)
        {
            await base.ApplySearch(searchFor);
            await Refresh();
        }

        private IQueryable<IPackage> GetBaseQuery()
        {
            return controller.Repo.GetPackages().Where(p => p.IsLatestVersion);
        }
    }
}
