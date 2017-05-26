using System;
using HotChocolatey.Utility;

namespace HotChocolatey.Model.ChocoTask
{
    internal class UninstallChocoTask : BaseChocoTask
    {
        private readonly Action<string> outputLineCallback;
        private readonly Package package;

        public UninstallChocoTask(Action<string> outputLineCallback, Package package)
        {
            Log.Info($"{nameof(UninstallChocoTask)}: {package.Id}");

            this.outputLineCallback = outputLineCallback;
            this.package = package;

            Config.CommandName = "uninstall";
            Config.PromptForConfirmation = false;
            Config.PackageNames = package.Id;
        }

        protected override Action<string> GetOutputLineCallback() => outputLineCallback;

        protected override void AfterExecute(bool result)
        {
            if (!result)
            {
                Log.Error($"{nameof(UninstallChocoTask)} failed for the following package: {package.Id}");
            }
        }
    }
}