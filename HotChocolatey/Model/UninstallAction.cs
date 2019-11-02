using System;
using NuGet;

namespace HotChocolatey.Model
{
    public class UninstallAction : IAction
    {
        private readonly Package[] packages;

        public UninstallAction(Package package)
        {
            this.packages = new[] { package };
        }

        public UninstallAction(Package[] packages)
        {
            this.packages = packages;
        }

        public void Execute(ChocoExecutor chocoExecutor, Action<string> outputLineCallback)
        {
            chocoExecutor.Uninstall(packages, outputLineCallback);
        }
    }
}