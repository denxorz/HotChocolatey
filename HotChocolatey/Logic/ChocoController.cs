using HotChocolatey.Utility;
using NuGet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HotChocolatey.Logic
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

        public async Task<List<ChocoItem>> GetAvailable(string name, UI.ProgressIndication.IProgressIndicator progressIndicator)
        {
            var packages = (await GetPackages(name)).Select(t => new ChocoItem(t)).ToList();
            await Task.WhenAll(packages.Select(UpdatePackageVersion));
            packages.ForEach(t => t.Actions = ActionFactory.Generate(this, t, progressIndicator));

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

        public async Task<List<ChocoItem>> GetInstalled(UI.ProgressIndication.IProgressIndicator progressIndicator)
        {
            var result = await Execute("upgrade all -r --whatif");
            result.ThrowIfNotSucceeded();

            var tasks = result.Output.Select(t =>
            {
                var tmp = t.Split(Seperator);
                return Task.Run(() => new ChocoItem(repo.FindPackage(tmp[0]), new SemanticVersion(tmp[1]), new SemanticVersion(tmp[2])));
            }).ToList();

            var packages = (await Task.WhenAll(tasks)).ToList();

            await Task.WhenAll(packages.Select(UpdatePackageVersion));
            packages.ForEach(t => t.Actions = ActionFactory.Generate(this, t, progressIndicator));
            return packages;
        }

        public async Task<bool> Install(ChocoItem package, SemanticVersion specificVersion = null)
        {
            Log.Info($"{nameof(Install)}: {package.Name} version:{specificVersion}");

            var version = specificVersion != null ? $" --version={specificVersion}" : string.Empty;
            var result = await Execute($"install -r -y {package.Name} {version}");

            if (!result.Succeeded)
            {
                Log.Error($"{nameof(Install)} failed for the following package: {package.Name}");
                return false;
            }

            return true;
        }

        public async Task<bool> Upgrade(ChocoItem package)
        {
            Log.Info($"{nameof(Upgrade)}: {package.Name}");

            var result = await Execute($"upgrade -r -y {package.Name}");

            if (!result.Succeeded)
            {
                Log.Error($"{nameof(Upgrade)} failed for the following package: {package.Name}");
                return false;
            }

            return true;
        }

        public async Task<bool> Uninstall(ChocoItem package)
        {
            Log.Info($"{nameof(Uninstall)}: {package.Name}");

            var result = await Execute($"uninstall -r -y {package.Name}");

            if (!result.Succeeded)
            {
                Log.Error($"{nameof(Uninstall)} failed for the following package: {package.Name}");
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

        private async Task UpdatePackageVersion(ChocoItem package)
        {
            package.Versions = await GetVersions(package.Package.Id);
        }

        private string AggregatePackageNames(List<ChocoItem> packages) => packages.Select(t => t.Name).Aggregate((all, next) => next + ";" + all);
    }
}
