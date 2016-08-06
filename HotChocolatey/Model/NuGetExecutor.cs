using HotChocolatey.Utility;
using NuGet;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HotChocolatey.Model
{
    public class NuGetExecutor
    {
        private IPackageRepository Repo { get; } = new PackageRepositoryFactory().CreateRepository("https://chocolatey.org/api/v2/");

        public void Update(Package package)
        {
            Log.Info($"NuGetExecutor update:{package.Id}");
            Add(package, Repo.FindPackage(package.Id));
        }

        private void Add(Package package, IPackage nugetPackage)
        {
            package.NugetPackage = nugetPackage;
        }

        public IQueryable<IPackage> GetPackages(System.Linq.Expressions.Expression<Func<IPackage, bool>> predicate)
        {
            return Repo.GetPackages().Where(predicate);
        }

        public async Task GetVersionAsync(Package package)
        {
            if (!package.Versions.Any())
            {
                var versions = await Task.Run(() => Repo.FindPackagesById(package.Id).Where(p => p.IsReleaseVersion()).Select(p => p.Version).ToList());
                package.Versions.AddRange(versions);
                package.Versions.Sort();
                package.UpdateLatestVersion();
                package.GenerateActions();
            }
        }
    }
}
