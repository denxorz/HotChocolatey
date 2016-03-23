using HotChocolatey.Utility;
using NuGet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HotChocolatey.Model
{
    public class ChocoExecutor
    {
        private readonly PackageRepo repo;
        private readonly NuGetExecutor nuGetExecutor;

        public List<Package> LocalPackages { get; } = new List<Package>();

        public ChocoExecutor(PackageRepo repo, NuGetExecutor nuGetExecutor)
        {
            this.repo = repo;
            this.nuGetExecutor = nuGetExecutor;
        }

        public async Task Update()
        {
            LocalPackages.Clear();
            repo.ClearLocalVersions();
            await UpdateLocal();
            await UpdateOutdated();
            await UpdateNuGetInfo();
        }

        public async Task Install(Package package, SemanticVersion specificVersion, Action<string> outputLineCallback)
        {
            Log.Info($"{nameof(Install)}: {package.Id} version:{specificVersion}");

            var version = specificVersion != null ? $" --version={specificVersion}" : string.Empty;
            var result = await Execute($"install -r -y {package.Id} {version}", outputLineCallback);

            if (!result)
            {
                Log.Error($"{nameof(Install)} failed for the following package: {package.Id}");
            }
        }

        public async Task Uninstall(Package package, Action<string> outputLineCallback)
        {
            Log.Info($"{nameof(Uninstall)}: {package.Id}");

            var result = await Execute($"uninstall -r -y {package.Id}", outputLineCallback);

            if (!result)
            {
                Log.Error($"{nameof(Uninstall)} failed for the following package: {package.Id}");
            }
        }

        public async Task Upgrade(Package package, SemanticVersion specificVersion, Action<string> outputLineCallback)
        {
            Log.Info($"{nameof(Upgrade)}: {package.Id} version:{specificVersion}");

            var version = specificVersion != null ? $" --version={specificVersion}" : string.Empty;
            var result = await Execute($"upgrade -r -y {package.Id} {version}", outputLineCallback);

            if (!result)
            {
                Log.Error($"{nameof(Upgrade)} failed for the following package: {package.Id}");
            }
        }

        private async Task UpdateLocal()
        {
            await Execute("list -r -l", UpdateLocalPackage);
        }

        private void UpdateLocalPackage(string chocoOutput)
        {
            var tmp = chocoOutput.Split('|');
            var p = repo.GetPackage(tmp[0]);
            p.InstalledVersion = new SemanticVersion(tmp[1]);
            LocalPackages.Add(p);
        }

        private async Task UpdateOutdated()
        {
            await Execute("outdated -r", UpdateOutdatedFlag);
        }

        private void UpdateOutdatedFlag(string chocoOutput)
        {
            var tmp = chocoOutput.Split('|');
            repo.GetPackage(tmp[0]).IsUpgradable = true;
        }

        private async Task UpdateNuGetInfo()
        {
            await Task.WhenAll(LocalPackages.Select(t => Task.Run(() => nuGetExecutor.Update(t))));
        }

        private async Task<bool> Execute(string arguments, Action<string> outputLineCallback)
        {
            Log.Info($">> choco {arguments}");

            using (Process choco = new Process())
            {
                choco.StartInfo.FileName = "choco";
                choco.StartInfo.Arguments = arguments;
                choco.StartInfo.UseShellExecute = false;
                choco.StartInfo.RedirectStandardOutput = true;
                choco.StartInfo.CreateNoWindow = true;
                choco.Start();

                bool endOfStream = false;
                while (!endOfStream)
                {
                    string line = await choco.StandardOutput.ReadLineAsync();
                    Log.Info($"> {line}");
                    outputLineCallback(line);

                    await Task.Run(() => endOfStream = choco.StandardOutput.EndOfStream);
                }

                if (choco.ExitCode != 0)
                {
                    Log.Error($">> choco {arguments} exited with code {choco.ExitCode}");
                }
                return choco.ExitCode == 0;
            }
        }
    }
}
