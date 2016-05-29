using HotChocolatey.Utility;
using NuGet;

namespace HotChocolatey.Model.ChocoTask
{
    internal class InstallChocoTask : BaseChocoTask
    {
        private readonly bool includePreReleases;
        private readonly Package package;
        private readonly SemanticVersion specificVersion;

        public InstallChocoTask(bool includePreReleases, Package package, SemanticVersion specificVersion)
        {
            Log.Info($"{nameof(InstallChocoTask)}: {package.Id} version:{specificVersion}");

            this.includePreReleases = includePreReleases;
            this.package = package;
            this.specificVersion = specificVersion;
        }

        protected override string GetCommand()
        {
            return "install";
        }

        protected override string GetParameters()
        {
            var version = specificVersion != null ? $" --version={specificVersion}" : string.Empty;
            var includePreRelease = includePreReleases ? "--prerelease" : string.Empty;
            return $"--yes {package.Id} {version} {includePreRelease}";
        }

        protected override void AfterExecute(bool result)
        {
            if (!result)
            {
                Log.Error($"{nameof(InstallChocoTask)} failed for the following package: {package.Id}");
            }
        }
    }
}