using System;
using HotChocolatey.Utility;
using NuGet;

namespace HotChocolatey.Model.ChocoTask
{
    internal class UpgradeChocoTask : BaseChocoTask
    {
        private readonly Action<string> outputLineCallback;
        private readonly string packageId;

        public UpgradeChocoTask(Action<string> outputLineCallback, bool includePreReleases, string packageId, SemanticVersion specificVersion)
        {
            Log.Info($"{nameof(UpgradeChocoTask)}: {packageId} version:{specificVersion}");

            this.outputLineCallback = outputLineCallback;
            this.packageId = packageId;

            Config.CommandName = "upgrade";
            Config.Version = specificVersion != null ? $"{specificVersion}" : string.Empty;
            Config.Prerelease = includePreReleases;
            Config.PackageNames = packageId;
            Config.PromptForConfirmation = false;
            Config.AllowDowngrade = true;
        }

        protected override Action<string> GetOutputLineCallback() => outputLineCallback;

        protected override void AfterExecute(bool result)
        {
            if (!result)
            {
                Log.Error($"{nameof(UpgradeChocoTask)} failed for the following package: {packageId}");
            }
        }
    }
}