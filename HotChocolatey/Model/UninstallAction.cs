using System;
using System.Threading.Tasks;
using NuGet;

namespace HotChocolatey.Model
{
    public class UninstallAction : IAction
    {
        private readonly Package package;

        public UninstallAction(Package package)
        {
            this.package = package;
        }

        public async Task ExecuteAsync(ChocoExecutor chocoExecutor, SemanticVersion specificVersion, Action<string> outputLineCallback)
        {
            await chocoExecutor.UninstallAsync(package, outputLineCallback);
        }
    }
}