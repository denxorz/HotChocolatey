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
            return new Version((await Execute(string.Empty))[0].Replace("Chocolatey v", string.Empty));
        }

        public async Task<List<ChocoItem>> GetInstalled()
        {
            return (await Execute("list -l -r")).Select(t => ChocoItem.FromInstalledString(t)).ToList();
        }

        public async Task<List<ChocoItem>> GetAvailable()
        {
            // TODO: remove proc (using for test speed)
            return (await Execute("list proc -r")).Select(t => ChocoItem.FromAvailableString(t)).ToList();
        }

        public async Task<List<ChocoItem>> GetUpgradable()
        {
            return (await Execute("upgrade all -r --whatif")).Select(t => ChocoItem.FromUpdatableString(t)).ToList();
        }

        public async Task Install(List<ChocoItem> packages)
        {
            if (packages.Count == 0) return;
            await Execute($"install {AggregatePackageNames(packages)} -r -y");
            // TODO: check return state
            packages.ForEach(t => t.IsMarkedForInstallation = false);
        }

        public async Task Upgrade(List<ChocoItem> packages)
        {
            if (packages.Count == 0) return;
            await Execute($"upgrade {AggregatePackageNames(packages)} -r -y");
            // TODO: check return state
            packages.ForEach(t => t.IsMarkedForUpgrade = false);
        }

        private async Task<List<string>> Execute(string arguments)
        {
            Output.Add($"> choco {arguments}");

            Process compiler = new Process();
            compiler.StartInfo.FileName = "choco";
            compiler.StartInfo.Arguments = arguments;
            compiler.StartInfo.UseShellExecute = false;
            compiler.StartInfo.RedirectStandardOutput = true;
            compiler.StartInfo.CreateNoWindow = true;
            compiler.Start();

            string output = await compiler.StandardOutput.ReadToEndAsync();

            compiler.WaitForExit();

            List<string> lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            lines.ForEach(Output.Add);

            return lines;
        }

        private string AggregatePackageNames(List<ChocoItem> packages) => packages.Select(t => t.Name).Aggregate((all, next) => next + ";" + all);
    }
}
