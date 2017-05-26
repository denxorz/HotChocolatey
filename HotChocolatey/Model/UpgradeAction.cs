using System;
using NuGet;

namespace HotChocolatey.Model
{
    public class UpgradeAction : IAction
    {
        private readonly Package package;

        public UpgradeAction(Package package)
        {
            this.package = package;
        }

        public void Execute(ChocoExecutor chocoExecutor, SemanticVersion specificVersion, Action<string> outputLineCallback)
        {
            chocoExecutor.Upgrade(package, specificVersion, outputLineCallback);
        }
    }
}