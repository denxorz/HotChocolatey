using HotChocolatey.ViewModel;
using NuGet;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotChocolatey.Model
{
    public class AllPackageList : IPackageList
    {
        private readonly ChocolateyController controller;

        private IOrderedQueryable<IPackage> query;
        private int skipped;
        private int total;
        private string searchFor;

        public bool HasMore => total > skipped;

        public AllPackageList(ChocolateyController controller)
        {
            this.controller = controller;
        }

        public async Task Refresh()
        {
            skipped = 0;
            await Task.Run(() =>
            {
                var baseQuery = GetBaseQuery();
                var includeSearch = string.IsNullOrWhiteSpace(searchFor) ? baseQuery : baseQuery.Where(t => t.Title.Contains(searchFor));
                query = includeSearch.OrderByDescending(p => p.DownloadCount);
            }).ContinueWith(task => total = query.Count());
        }

        public async Task<IEnumerable<ChocoItem>> GetMore(int numberOfItems)
        {
            var tmp = query.Skip(skipped).Take(numberOfItems).ToList();
            skipped += numberOfItems;

            var packages = tmp.Select(t => new ChocoItem(t)).ToList();
            await Task.WhenAll(packages.Select(controller.UpdatePackageVersion));
            packages.ForEach(t => t.Actions = ActionFactory.Generate(controller, t));

            return packages;
        }

        public async Task ApplySearch(string searchFor)
        {
            this.searchFor = searchFor;
            skipped = 0;
            await Refresh();
        }

        private IQueryable<IPackage> GetBaseQuery()
        {
            return controller.Repo.GetPackages().Where(p => p.IsLatestVersion);
        }
    }
}
