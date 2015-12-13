using HotChocolatey.ViewModel;
using NuGet;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace HotChocolatey.Model
{
    public static class PackageDisplayTypeFactory
    {
        public static List<IPackageDisplayType> BuildDisplayTypes(PackageRepo repo, NuGetExecutor nugetExecutor, ChocoExecutor chocoExecutor) =>
            new List<IPackageDisplayType>
            {
                new AllPackageDisplayType(repo, nugetExecutor, chocoExecutor),
                new InstalledPackageDisplayType(repo, nugetExecutor, chocoExecutor),
                new UpgradeablePackageDisplayType(repo, nugetExecutor, chocoExecutor),
            };

        public static IPackageDisplayType BuildUpgradeFilter(PackageRepo repo, NuGetExecutor nugetExecutor, ChocoExecutor chocoExecutor)
            => new UpgradeablePackageDisplayType(repo, nugetExecutor, chocoExecutor);

        private class AllPackageDisplayType : IPackageDisplayType
        {
            private readonly NuGetExecutor nugetExecutor;
            private readonly PackageRepo repo;

            private IOrderedQueryable<IPackage> query;
            private int total;
            private int skipped;
            private string searchFor;

            public bool HasMore => total > skipped;

            public AllPackageDisplayType(PackageRepo repo, NuGetExecutor nugetExecutor, ChocoExecutor chocoExecutor)
            {
                this.repo = repo;
                this.nugetExecutor = nugetExecutor;
            }

            public override string ToString() => "All";

            public async Task Refresh()
            {
                skipped = 0;
                await Task.Run(() =>
                {
                    var baseQuery = GetBaseQuery();

                    // Query does not support this
#pragma warning disable S1449 // Culture should be specified for String operations
                    var includeSearch = string.IsNullOrWhiteSpace(searchFor)
                                                ? baseQuery
                                                : baseQuery.Where(t => t.Tags.ToLower().Contains(searchFor)
                                                                    || t.Title.ToLower().Contains(searchFor));
#pragma warning restore S1449 // Culture should be specified for String operations

                    query = includeSearch.OrderByDescending(p => p.DownloadCount);
                }).ContinueWith(task => total = query.Count());
            }

            public async Task<IEnumerable<Package>> GetMore(int numberOfItems)
            {
                var tmp = query.Skip(skipped).Take(numberOfItems).ToList();
                skipped += numberOfItems;

                var packages = tmp.Select(t =>
                {
                    var p = repo.GetPackage(t.Id);
                    p.NugetPackage = t;
                    return p;
                }).ToList();

                return packages;
            }

            public async Task ApplySearch(string search)
            {
                searchFor = search.ToLower(CultureInfo.InvariantCulture);
                await Refresh();
            }

            private IQueryable<IPackage> GetBaseQuery()
            {
                return nugetExecutor.GetPackages(p => p.IsLatestVersion);
            }
        }

        private class InstalledPackageDisplayType : IPackageDisplayType
        {
            private readonly ChocoExecutor chocoExecutor;

            private int skipped;
            private string searchFor;

            public bool HasMore => chocoExecutor.LocalPackages.Count > skipped;

            public InstalledPackageDisplayType(PackageRepo repo, NuGetExecutor controller, ChocoExecutor chocoExecutor)
            {
                this.chocoExecutor = chocoExecutor;
            }

            public override string ToString() => "Installed";

            public async Task Refresh()
            {
                skipped = 0;
            }

            public async Task<IEnumerable<Package>> GetMore(int numberOfItems)
            {
                var searchedPackages = string.IsNullOrWhiteSpace(searchFor)
                    ? chocoExecutor.LocalPackages
                    : chocoExecutor.LocalPackages.Where(p => PackageSearchComparer(p, searchFor));
                var tmp = searchedPackages.Skip(skipped).Take(numberOfItems);
                skipped += numberOfItems;
                return tmp;
            }

            private bool PackageSearchComparer(Package package, string search)
            {
                return package.Tags?.Contains(search) == true || package.Title.Contains(search);
            }

            public async Task ApplySearch(string search)
            {
                searchFor = search.ToLower(CultureInfo.InvariantCulture);
                await Refresh();
            }
        }

        private class UpgradeablePackageDisplayType : IPackageDisplayType
        {
            private readonly ChocoExecutor chocoExecutor;

            private int skipped;
            private string searchFor;

            public bool HasMore => chocoExecutor.LocalPackages.Count > skipped;

            public UpgradeablePackageDisplayType(PackageRepo repo, NuGetExecutor controller, ChocoExecutor chocoExecutor)
            {
                this.chocoExecutor = chocoExecutor;
            }

            public override string ToString() => "Upgrade available";

            public async Task Refresh()
            {
                skipped = 0;
            }

            public async Task<IEnumerable<Package>> GetMore(int numberOfItems)
            {
                var searchedPackages = string.IsNullOrWhiteSpace(searchFor)
                    ? chocoExecutor.LocalPackages.Where(p => p.IsUpgradable)
                    : chocoExecutor.LocalPackages.Where(p => PackageSearchComparer(p, searchFor) && p.IsUpgradable);
                var tmp = searchedPackages.Skip(skipped).Take(numberOfItems);
                skipped += numberOfItems;
                return tmp;
            }

            private bool PackageSearchComparer(Package package, string search)
            {
                return package.Tags.Contains(search) || package.Title.Contains(search);
            }

            public async Task ApplySearch(string search)
            {
                searchFor = search.ToLower(CultureInfo.InvariantCulture);
                await Refresh();
            }
        }
    }
}
