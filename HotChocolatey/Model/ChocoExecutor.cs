using HotChocolatey.Utility;
using NuGet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HotChocolatey.Model.ChocoTask;

namespace HotChocolatey.Model
{
    public class ChocoExecutor
    {
        private readonly PackageRepo repo;
        private readonly NuGetExecutor nuGetExecutor;

        public List<Package> LocalPackages { get; } = new List<Package>();
        public bool IncludePreReleases { get; set; }

        public ChocoExecutor(PackageRepo repo, NuGetExecutor nuGetExecutor)
        {
            this.repo = repo;
            this.nuGetExecutor = nuGetExecutor;
        }

        public async Task Update()
        {
            LocalPackages.Clear();
            repo.ClearLocalVersions();
            await new UpdateLocalChocoTask(IncludePreReleases).Execute(this, UpdateLocalPackage);
            await new UpdateOutdatedFlagsChocoTask().Execute(this, UpdateOutdatedFlag);
            await UpdateNuGetInfo();
        }

        public async Task Install(Package package, SemanticVersion specificVersion, Action<string> outputLineCallback)
        {
            await new InstallChocoTask(IncludePreReleases, package, specificVersion).Execute(this, outputLineCallback);
        }

        public async Task Uninstall(Package package, Action<string> outputLineCallback)
        {
            await new UninstallChocoTask(package).Execute(this, outputLineCallback);
        }

        public async Task Upgrade(Package package, SemanticVersion specificVersion, Action<string> outputLineCallback)
        {
            await new UpgradeChocoTask(IncludePreReleases, package, specificVersion).Execute(this, outputLineCallback);
        }

        private void UpdateLocalPackage(string chocoOutput)
        {
            var tmp = chocoOutput.Split('|', ' ');
            var p = repo.GetPackage(tmp[0]);
            p.InstalledVersion = new SemanticVersion(tmp[1]);
            LocalPackages.Add(p);
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

        public async Task<bool> Execute(string arguments, Action<string> outputLineCallback)
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
