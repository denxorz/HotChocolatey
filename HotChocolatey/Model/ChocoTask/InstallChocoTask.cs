using System;
using HotChocolatey.Utility;
using NuGet;

namespace HotChocolatey.Model.ChocoTask
{
    internal class InstallChocoTask : BaseChocoTask
    {
        private readonly Action<string> outputLineCallback;
        private readonly string packageId;

        protected override Action<string> GetOutputLineCallback() => outputLineCallback;

        public InstallChocoTask(Action<string> outputLineCallback, bool includePreReleases, string packageId, SemanticVersion specificVersion)
        {
            Log.Info($"{nameof(InstallChocoTask)}: {packageId} version:{specificVersion}");

            this.outputLineCallback = outputLineCallback;
            this.packageId = packageId;

            Config.CommandName = "install";
            Config.Version = specificVersion != null ? $"{specificVersion}" : string.Empty;
            Config.Prerelease = includePreReleases;
            Config.PromptForConfirmation = false;
            Config.PackageNames = packageId;
        }

        protected override void AfterExecute(bool result)
        {
            if (!result)
            {
                Log.Error($"{nameof(InstallChocoTask)} failed for the following package: {packageId}");
            }
        }
    }
}