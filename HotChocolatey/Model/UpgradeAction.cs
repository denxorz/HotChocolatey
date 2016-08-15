using System;
using System.Threading.Tasks;
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

        public async Task ExecuteAsync(ChocoExecutor chocoExecutor, SemanticVersion specificVersion, Action<string> outputLineCallback)
        {
            await chocoExecutor.UpgradeAsync(package, specificVersion, outputLineCallback);
        }
    }
}