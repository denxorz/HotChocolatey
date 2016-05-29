using HotChocolatey.Utility;
using NuGet;

namespace HotChocolatey.Model.ChocoTask
{
    internal class UpgradeChocoTask : BaseChocoTask
    {
        private readonly bool includePreReleases;
        private readonly Package package;
        private readonly SemanticVersion specificVersion;

        public UpgradeChocoTask(bool includePreReleases, Package package, SemanticVersion specificVersion)
        {
            Log.Info($"{nameof(UpgradeChocoTask)}: {package.Id} version:{specificVersion}");

            this.includePreReleases = includePreReleases;
            this.package = package;
            this.specificVersion = specificVersion;
        }

        protected override string GetCommand()
        {
            return "upgrade";
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
                Log.Error($"{nameof(UpgradeChocoTask)} failed for the following package: {package.Id}");
            }
        }
    }
}