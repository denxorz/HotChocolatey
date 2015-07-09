using NuGet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HotChocolatey
{
    public class ChocolateyController
    {
        private const char Seperator = '|';
        private PackageRepositoryFactory packageRepository = new PackageRepositoryFactory();
        private IPackageRepository repo;

        public ChocolateyController()
        {
            repo = packageRepository.CreateRepository("https://chocolatey.org/api/v2/");
        }

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

            var tasks = result.Output.Select(t =>
                {
                    var tmp = t.Split(Seperator);
                    return Task.Run(() => ChocoItem.FromInstalledString(repo.FindPackage(tmp[0]), tmp[1]));
                }).ToList();

            var packages = (await Task.WhenAll(tasks)).ToList();
            packages.ForEach(async t => t.Versions = await GetVersions(t.Package.Id));

            return packages;
        }

        public async Task<List<ChocoItem>> GetAvailable(string name)
        {
            var packages = (await GetPackages(name)).Select(t => ChocoItem.FromPackage(t)).ToList();
            packages.ForEach(async t => t.Versions = await GetVersions(t.Package.Id));

            return packages;
        }

        public async Task<List<IPackage>> GetPackages(string name)
        {
            return await Task.Run(() => repo.GetPackages().Where(p => p.Title.Contains(name) && p.IsLatestVersion).ToList());
        }

        public async Task<List<SemanticVersion>> GetVersions(string id)
        {
            return await Task.Run(() => repo.GetPackages().Where(p => p.Id == id).ToList().Select(p => p.Version).OrderByDescending(p => p.Version).ToList());
        }

        public async Task<List<ChocoItem>> GetUpgradable()
        {
            var result = await Execute("upgrade all -r --whatif");
            result.ThrowIfNotSucceeded();

            var packages = await Task.Run(() => result.Output.Select(t =>
             {
                 var tmp = t.Split(Seperator);
                 return ChocoItem.FromUpdatableString(repo.FindPackage(tmp[0]), tmp[1], tmp[2]);
             }).ToList());
            packages.ForEach(async t => t.Versions = await GetVersions(t.Package.Id));

            return packages;
        }

        public async Task<bool> Install(List<ChocoItem> packages)
        {
            string packagesToInstall = AggregatePackageNames(packages);
            Log.Info($"{nameof(Install)}: {packagesToInstall}");

            if (packages.Count == 0) return true;

            var result = await Execute($"install {packagesToInstall} -r -y");

            if (!result.Succeeded)
            {
                Log.Error($"{nameof(Install)} failed for the following packages: {packagesToInstall}");
                return false;
            }

            return true;
        }

        public async Task<bool> Upgrade(List<ChocoItem> packages)
        {
            string packagesToUpgrade = AggregatePackageNames(packages);
            Log.Info($"{nameof(Upgrade)}: {packagesToUpgrade}");

            if (packages.Count == 0) return true;

            var result = await Execute($"upgrade {packagesToUpgrade} -r -y");

            if (!result.Succeeded)
            {
                Log.Error($"{nameof(Upgrade)} failed for the following packages: {packagesToUpgrade}");
                return false;
            }

            return true;
        }

        public async Task<bool> Uninstall(List<ChocoItem> packages)
        {
            string packagesToUninstall = AggregatePackageNames(packages);
            Log.Info($"{nameof(Uninstall)}: {packagesToUninstall}");

            if (packages.Count == 0) return true;

            var result = await Execute($"uninstall {packagesToUninstall} -r -y");

            if (!result.Succeeded)
            {
                Log.Error($"{nameof(Uninstall)} failed for the following packages: {packagesToUninstall}");
                return false;
            }

            return true;
        }

        private async Task<ChocolateyResult> Execute(string arguments)
        {
            Log.Info($">> choco {arguments}");

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
            result.Output.ForEach(t => Log.Info($"> {t}"));

            return result;
        }

        private string AggregatePackageNames(List<ChocoItem> packages) => packages.Select(t => t.Name).Aggregate((all, next) => next + ";" + all);
    }
}
