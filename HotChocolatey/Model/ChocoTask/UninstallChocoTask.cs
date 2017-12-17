using System;
using HotChocolatey.Utility;

namespace HotChocolatey.Model.ChocoTask
{
    internal class UninstallChocoTask : BaseChocoTask
    {
        private readonly Action<string> outputLineCallback;
        private readonly string packageId;

        public UninstallChocoTask(Action<string> outputLineCallback, string packageId)
        {
            Log.Info($"{nameof(UninstallChocoTask)}: {packageId}");

            this.outputLineCallback = outputLineCallback;
            this.packageId = packageId;

            Config.CommandName = "uninstall";
            Config.PromptForConfirmation = false;
            Config.PackageNames = packageId;
        }

        protected override Action<string> GetOutputLineCallback() => outputLineCallback;

        protected override void AfterExecute(bool result)
        {
            if (!result)
            {
                Log.Error($"{nameof(UninstallChocoTask)} failed for the following package: {packageId}");
            }
        }
    }
}