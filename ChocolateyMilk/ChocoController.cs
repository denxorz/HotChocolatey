using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ChocolateyMilk
{
    public class ChocolateyController
    {
        public const char Seperator = '|';

        public ObservableCollection<string> Output { get; } = new ObservableCollection<string>();

        public async Task<Version> GetVersion()
        {
            var result = await Execute(string.Empty);

            if (result.ExitCode != 1) throw new ChocoExecutionException(result);

            return new Version(result.Output.First().Replace("Chocolatey v", string.Empty));
        }

        public async Task<List<ChocoItem>> GetInstalled()
        {
            var result = await Execute("list -l -r");
            result.ThrowIfNotSucceeded();

            return result.Output.Select(t => ChocoItem.FromInstalledString(t)).ToList();
        }

        public async Task<List<ChocoItem>> GetAvailable(string name)
        {
            var result = await Execute($"list {name} -r");
            result.ThrowIfNotSucceeded();

            return result.Output.Select(t => ChocoItem.FromAvailableString(t)).ToList();
        }

        public async Task<List<ChocoItem>> GetUpgradable()
        {
            var result = await Execute("upgrade all -r --whatif");
            result.ThrowIfNotSucceeded();

            return result.Output.Select(t => ChocoItem.FromUpdatableString(t)).ToList();
        }

        public async Task<bool> Install(List<ChocoItem> packages)
        {
            if (packages.Count == 0) return true;

            var result = await Execute($"install {AggregatePackageNames(packages)} -r -y");

            if (!result.Succeeded) return false;

            packages.ForEach(t => t.IsMarkedForInstallation = false);
            return true;
        }

        public async Task<bool> Upgrade(List<ChocoItem> packages)
        {
            if (packages.Count == 0) return true;

            var result = await Execute($"upgrade {AggregatePackageNames(packages)} -r -y");

            if (!result.Succeeded) return false;

            packages.ForEach(t => t.IsMarkedForUpgrade = false);
            return true;
        }

        public async Task<bool> Uninstall(List<ChocoItem> packages)
        {
            if (packages.Count == 0) return true;

            var result = await Execute($"uninstall {AggregatePackageNames(packages)} -r -y");

            if (!result.Succeeded) return false;

            packages.ForEach(t => t.IsMarkedForUninstall = false);
            return true;
        }

        private async Task<ChocolateyResult> Execute(string arguments)
        {
            Output.Add($"> choco {arguments}");

            Process choco = new Process();
            choco.StartInfo.FileName = "choco";
            choco.StartInfo.Arguments = arguments;
            choco.StartInfo.UseShellExecute = false;
            choco.StartInfo.RedirectStandardOutput = true;
            choco.StartInfo.CreateNoWindow = true;
            choco.Start();

            string output = await choco.StandardOutput.ReadToEndAsync();

            choco.WaitForExit();

            var result = new ChocolateyResult(output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList(), choco.ExitCode, arguments);
            result.Output.ForEach(Output.Add);

            return result;
        }

        private string AggregatePackageNames(List<ChocoItem> packages) => packages.Select(t => t.Name).Aggregate((all, next) => next + ";" + all);
    }
}
