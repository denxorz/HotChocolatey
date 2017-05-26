using System;
using NuGet;

namespace HotChocolatey.Model
{
    public class InstallAction : IAction
    {
        private readonly Package package;

        public InstallAction(Package package)
        {
            this.package = package;
        }

        public void Execute(ChocoExecutor chocoExecutor, SemanticVersion specificVersion, Action<string> outputLineCallback)
        {
            chocoExecutor.Install(package, specificVersion, outputLineCallback);
        }
    }
}