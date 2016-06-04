using NuGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolatey.Model.ChocoTask;

namespace HotChocolatey.Model
{
    public class ChocoExecutor
    {
        public List<Package> LocalPackages { get; } = new List<Package>();
        public bool IncludePreReleases { get; set; }

        public async Task Update(PackageRepo repo, NuGetExecutor nuGetExecutor)
        {
            LocalPackages.Clear();
            repo.ClearLocalVersions();
            await new UpdateLocalChocoTask(IncludePreReleases, repo, LocalPackages.Add).Execute();
            await new UpdateOutdatedFlagsChocoTask(repo).Execute();
            await UpdateNuGetInfo(nuGetExecutor);
        }

        public async Task Install(Package package, SemanticVersion specificVersion, Action<string> outputLineCallback)
        {
            await new InstallChocoTask(outputLineCallback, IncludePreReleases, package, specificVersion).Execute();
        }

        public async Task Uninstall(Package package, Action<string> outputLineCallback)
        {
            await new UninstallChocoTask(outputLineCallback, package).Execute();
        }

        public async Task Upgrade(Package package, SemanticVersion specificVersion, Action<string> outputLineCallback)
        {
            await new UpgradeChocoTask(outputLineCallback, IncludePreReleases, package, specificVersion).Execute();
        }

        private async Task UpdateNuGetInfo(NuGetExecutor nuGetExecutor)
        {
            await Task.WhenAll(LocalPackages.Select(t => Task.Run(() => nuGetExecutor.Update(t))));
        }
    }
}
