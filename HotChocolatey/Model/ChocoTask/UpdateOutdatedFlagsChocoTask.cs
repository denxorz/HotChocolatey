using System;

namespace HotChocolatey.Model.ChocoTask
{
    internal class UpdateOutdatedFlagsChocoTask : BaseChocoTask
    {
        private readonly PackageRepo repo;

        public UpdateOutdatedFlagsChocoTask(PackageRepo repo)
        {
            this.repo = repo;
        }

        protected override string GetCommand() => "outdated";

        protected override string GetParameters() => string.Empty;

        protected override Action<string> GetOutputLineCallback() => UpdateOutdatedFlag;
        
        private void UpdateOutdatedFlag(string chocoOutput)
        {
            var tmp = chocoOutput.Split('|');
            repo.GetPackage(tmp[0]).IsUpgradable = true;
        }
    }
}