using System;
using System.Threading.Tasks;
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

        public async Task ExecuteAsync(ChocoExecutor chocoExecutor, SemanticVersion specificVersion, Action<string> outputLineCallback)
        {
            await chocoExecutor.InstallAsync(package, specificVersion, outputLineCallback);
        }
    }
}