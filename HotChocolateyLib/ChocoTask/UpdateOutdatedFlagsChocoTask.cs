using System;

namespace HotChocolatey.Model.ChocoTask
{
    public class UpdateOutdatedFlagsChocoTask : BaseChocoTask
    {
        private readonly PackageRepo repo;

        public UpdateOutdatedFlagsChocoTask(PackageRepo repo)
        {
            this.repo = repo;

            Config.CommandName = "outdated";
        }

        protected override Action<string> GetOutputLineCallback() => UpdateOutdatedFlag;

        private void UpdateOutdatedFlag(string chocoOutput)
        {
            var tmp = chocoOutput.Split('|');
            repo.GetPackage(tmp[0]).IsUpgradable = true;
        }
    }
}