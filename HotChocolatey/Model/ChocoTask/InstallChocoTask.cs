using System;
using HotChocolatey.Utility;
using NuGet;

namespace HotChocolatey.Model.ChocoTask
{
    internal class InstallChocoTask : BaseChocoTask
    {
        private readonly Action<string> outputLineCallback;
        private readonly Package package;

        protected override Action<string> GetOutputLineCallback() => outputLineCallback;

        public InstallChocoTask(Action<string> outputLineCallback, bool includePreReleases, Package package, SemanticVersion specificVersion)
        {
            Log.Info($"{nameof(InstallChocoTask)}: {package.Id} version:{specificVersion}");

            this.outputLineCallback = outputLineCallback;
            this.package = package;

            Config.CommandName = "install";
            Config.Version = specificVersion != null ? $"{specificVersion}" : string.Empty;
            Config.Prerelease = includePreReleases;
            Config.PromptForConfirmation = false;
            Config.PackageNames = package.Id;
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