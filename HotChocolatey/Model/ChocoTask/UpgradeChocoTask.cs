using System;
using HotChocolatey.Utility;
using NuGet;

namespace HotChocolatey.Model.ChocoTask
{
    internal class UpgradeChocoTask : BaseChocoTask
    {
        private readonly Action<string> outputLineCallback;
        private readonly Package package;

        public UpgradeChocoTask(Action<string> outputLineCallback, bool includePreReleases, Package package, SemanticVersion specificVersion)
        {
            Log.Info($"{nameof(UpgradeChocoTask)}: {package.Id} version:{specificVersion}");

            this.outputLineCallback = outputLineCallback;
            this.package = package;

            Config.CommandName = "upgrade";
            Config.Version = specificVersion != null ? $"{specificVersion}" : string.Empty;
            Config.Prerelease = includePreReleases;
            Config.PackageNames = package.Id;
            Config.PromptForConfirmation = false;
            Config.AllowDowngrade = true;
        }

        protected override Action<string> GetOutputLineCallback() => outputLineCallback;

        protected override void AfterExecute(bool result)
        {
            if (!result)
            {
                Log.Error($"{nameof(UpgradeChocoTask)} failed for the following package: {package.Id}");
            }
        }
    }
}