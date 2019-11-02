using System;
using NuGet;

namespace HotChocolatey.Model
{
    public class UpgradeAction : IAction
    {
        private readonly Package[] packages;
        private readonly SemanticVersion specificVersion;

        public UpgradeAction(Package package, SemanticVersion specificVersion)
        {
            this.packages = new[] { package };
            this.specificVersion = specificVersion;
        }

        public UpgradeAction(Package[] packages)
        {
            this.packages = packages;
        }

        public void Execute(ChocoExecutor chocoExecutor, Action<string> outputLineCallback)
        {
            chocoExecutor.Upgrade(packages, specificVersion, outputLineCallback);
        }
    }
}