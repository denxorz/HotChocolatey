using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace WpfApplication1
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
            return (await Execute("list -r Atom")).Select(t => ChocoItem.FromAvailableString(t)).ToList();
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
    }
}
