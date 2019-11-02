using System;
using NuGet;

namespace HotChocolatey.Model
{
    public class InstallAction : IAction
    {
        private readonly Package[] packages;
        private readonly SemanticVersion specificVersion;

        public InstallAction(Package package, SemanticVersion specificVersion)
        {
            this.packages = new[] { package };
            this.specificVersion = specificVersion;
        }

        public InstallAction(Package[] packages)
        {
            this.packages = packages;
        }

        public void Execute(ChocoExecutor chocoExecutor, Action<string> outputLineCallback)
        {
            chocoExecutor.Install(packages, specificVersion, outputLineCallback);
        }
    }
}