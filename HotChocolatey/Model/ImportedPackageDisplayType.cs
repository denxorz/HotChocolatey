using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using HotChocolatey.ViewModel;

namespace HotChocolatey.Model
{
    public class ImportedPackageDisplayType : IPackageDisplayType
    {
        private readonly string name;
        private readonly NuGetExecutor controller;
        private readonly ChocoExecutor chocoExecutor;
        private readonly List<Package> importedPackages;

        private int skipped;
        private string searchFor;
        private IEnumerable<Package> query;

        public bool HasMore => chocoExecutor.LocalPackages.Count > skipped;

        public bool AllowsMultiSelect { get; } = true;
        public bool AllowsMultiSelectInstall { get; } = true;
        public bool AllowsMultiSelectUpdate { get; } = false;
        public bool AllowsMultiSelectUninstall { get; } = false;
        
        public ImportedPackageDisplayType(string name, PackageRepo repo, NuGetExecutor controller, ChocoExecutor chocoExecutor, IEnumerable<string> importedPackageIds)
        {
            this.name = name;
            this.controller = controller;
            this.chocoExecutor = chocoExecutor;
            importedPackages = importedPackageIds.Select(repo.GetPackage).ToList();
        }

        public override string ToString() => $"[import] {name}";

        public async Task RefreshAsync()
        {
            skipped = 0;

            query = string.IsNullOrWhiteSpace(searchFor)
                ? importedPackages
                : importedPackages.Where(p => PackageSearchComparer(p, searchFor));
        }

        public IEnumerable<Package> GetMore(int numberOfItems)
        {
            var packages = query.Skip(skipped).Take(numberOfItems).ToList();
            packages.ForEach(t => controller.Update(t));

            skipped += numberOfItems;

            return packages;
        }

        private bool PackageSearchComparer(Package package, string search)
        {
            return package.Tags?.Contains(search) == true || package.Title.Contains(search);
        }

        public async Task ApplySearchAsync(string search)
        {
            searchFor = search.ToLower(CultureInfo.InvariantCulture);
            await RefreshAsync();
        }
    }
}