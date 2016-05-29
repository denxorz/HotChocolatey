using HotChocolatey.Utility;

namespace HotChocolatey.Model.ChocoTask
{
    internal class UninstallChocoTask : BaseChocoTask
    {
        private readonly Package package;

        public UninstallChocoTask(Package package)
        {
            Log.Info($"{nameof(UninstallChocoTask)}: {package.Id}");

            this.package = package;
        }

        protected override string GetCommand()
        {
            return "uninstall";
        }

        protected override string GetParameters()
        {
            return $"--yes {package.Id}";
        }

        protected override void AfterExecute(bool result)
        {
            if (!result)
            {
                Log.Error($"{nameof(UninstallChocoTask)} failed for the following package: {package.Id}");
            }
        }
    }
}