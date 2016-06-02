using System;
using HotChocolatey.Utility;
using NuGet;

namespace HotChocolatey.Model.ChocoTask
{
    internal class InstallChocoTask : BaseChocoTask
    {
        private readonly Action<string> outputLineCallback;
        private readonly bool includePreReleases;
        private readonly Package package;
        private readonly SemanticVersion specificVersion;

        public InstallChocoTask(Action<string> outputLineCallback, bool includePreReleases, Package package, SemanticVersion specificVersion) 
        {
            Log.Info($"{nameof(InstallChocoTask)}: {package.Id} version:{specificVersion}");

            this.outputLineCallback = outputLineCallback;
            this.includePreReleases = includePreReleases;
            this.package = package;
            this.specificVersion = specificVersion;
        }

        protected override string GetCommand() => "install";
        
        protected override string GetParameters()
        {
            var version = specificVersion != null ? $" --version={specificVersion}" : string.Empty;
            var includePreRelease = includePreReleases ? "--prerelease" : string.Empty;
            return $"--yes {package.Id} {version} {includePreRelease}";
        }

        protected override Action<string> GetOutputLineCallback() => outputLineCallback;
        
        protected override void AfterExecute(bool result)
        {
            if (!result)
            {
                Log.Error($"{nameof(InstallChocoTask)} failed for the following package: {package.Id}");
            }
        }
    }
}