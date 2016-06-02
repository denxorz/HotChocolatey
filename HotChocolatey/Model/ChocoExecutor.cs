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
        public List<Package> LocalPackages { get; } = new List<Package>();
        public bool IncludePreReleases { get; set; }

        public async Task Update(PackageRepo repo, NuGetExecutor nuGetExecutor)
        {
            LocalPackages.Clear();
            repo.ClearLocalVersions();
            await new UpdateLocalChocoTask(IncludePreReleases, repo, LocalPackages.Add).Execute(this);
            await new UpdateOutdatedFlagsChocoTask(repo).Execute(this);
            await UpdateNuGetInfo(nuGetExecutor);
        }

        public async Task Install(Package package, SemanticVersion specificVersion, Action<string> outputLineCallback)
        {
            await new InstallChocoTask(outputLineCallback, IncludePreReleases, package, specificVersion).Execute(this);
        }

        public async Task Uninstall(Package package, Action<string> outputLineCallback)
        {
            await new UninstallChocoTask(outputLineCallback, package).Execute(this);
        }

        public async Task Upgrade(Package package, SemanticVersion specificVersion, Action<string> outputLineCallback)
        {
            await new UpgradeChocoTask(outputLineCallback, IncludePreReleases, package, specificVersion).Execute(this);
        }

        private async Task UpdateNuGetInfo(NuGetExecutor nuGetExecutor)
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
