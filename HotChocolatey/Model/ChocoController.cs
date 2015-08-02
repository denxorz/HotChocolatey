using HotChocolatey.Utility;
using HotChocolatey.ViewModel;
using NuGet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HotChocolatey.Model
{
    public interface IPackageList
    {
        bool HasMore { get; }
        Task Refresh();
        Task<IEnumerable<ChocoItem>> GetMore(int numberOfItems);
    }

    public class InstalledPackageList : IPackageList
    {
        private readonly ChocolateyController controller;

        private List<ChocoItem> packages = new List<ChocoItem>();
        private int skipped;

        public bool HasMore => packages.Count > skipped;

        public InstalledPackageList(ChocolateyController controller)
        {
            this.controller = controller;
        }

        public async Task Refresh()
        {
            skipped = 0;
            packages = controller.InstalledPackages;
        }

        public async Task<IEnumerable<ChocoItem>> GetMore(int numberOfItems)
        {
            var tmp = packages.Skip(skipped).Take(numberOfItems);
            skipped += numberOfItems;
            return tmp;
        }
    }

    public class UpgradablePackageList : IPackageList
    {
        private readonly ChocolateyController controller;

        private List<ChocoItem> packages = new List<ChocoItem>();
        private int skipped;

        public bool HasMore => packages.Count > skipped;

        public UpgradablePackageList(ChocolateyController controller)
        {
            this.controller = controller;
        }

        public async Task Refresh()
        {
            skipped = 0;
            packages = controller.InstalledPackages.Where(t => t.IsUpgradable).ToList();
        }

        public async Task<IEnumerable<ChocoItem>> GetMore(int numberOfItems)
        {
            var tmp = packages.Skip(skipped).Take(numberOfItems);
            skipped += numberOfItems;
            return tmp;
        }
    }

    public class SearchResultPackageList : IPackageList
    {
        private readonly ChocolateyController controller;
        private readonly ProgressIndication.IProgressIndicator progressIndicator;
        private readonly string searchText;

        private IOrderedQueryable<IPackage> query;
        private int skipped;
        private int total;

        public bool HasMore => total > skipped;

        public SearchResultPackageList(ChocolateyController controller, ProgressIndication.IProgressIndicator progressIndicator, string searchText)
        {
            this.controller = controller;
            this.progressIndicator = progressIndicator;
            this.searchText = searchText;
        }

        public async Task Refresh()
        {
            skipped = 0;
            query = await Task.Run(() => controller.Repo.GetPackages().Where(p => p.Title.Contains(searchText) && p.IsLatestVersion).OrderByDescending(p => p.DownloadCount));
            total = await Task.Run(() => query.Count());
        }

        public async Task<IEnumerable<ChocoItem>> GetMore(int numberOfItems)
        {
            var tmp = query.Skip(skipped).Take(numberOfItems).ToList();
            skipped += numberOfItems;

            var packages = tmp.Select(t => new ChocoItem(t)).ToList();
            await Task.WhenAll(packages.Select(controller.UpdatePackageVersion));
            packages.ForEach(t => t.Actions = ActionFactory.Generate(controller, t, progressIndicator));

            return packages;
        }
    }

    public class AllPackageList : IPackageList
    {
        private readonly ChocolateyController controller;
        private readonly ProgressIndication.IProgressIndicator progressIndicator;

        private IOrderedQueryable<IPackage> query;
        private int skipped;
        private int total;

        public bool HasMore => total > skipped;

        public AllPackageList(ChocolateyController controller, ProgressIndication.IProgressIndicator progressIndicator)
        {
            this.controller = controller;
            this.progressIndicator = progressIndicator;
        }

        public Task Refresh()
        {
            skipped = 0;
            return Task.Run(() =>
                {
                    query = controller.Repo.GetPackages().Where(p => p.IsLatestVersion).OrderByDescending(p => p.DownloadCount);
                    total = query.Count();
                });
        }

        public async Task<IEnumerable<ChocoItem>> GetMore(int numberOfItems)
        {
            var tmp = query.Skip(skipped).Take(numberOfItems).ToList();
            skipped += numberOfItems;

            var packages = tmp.Select(t => new ChocoItem(t)).ToList();
            await Task.WhenAll(packages.Select(controller.UpdatePackageVersion));
            packages.ForEach(t => t.Actions = ActionFactory.Generate(controller, t, progressIndicator));

            return packages;
        }
    }

    public class ChocolateyController
    {
        private const char Seperator = '|';

        public IPackageRepository Repo { get; } = new PackageRepositoryFactory().CreateRepository("https://chocolatey.org/api/v2/");
        public List<ChocoItem> InstalledPackages { get; private set; } = new List<ChocoItem>();

        public async Task<Version> GetVersion()
        {
            var result = await Execute(string.Empty);

            if (result.ExitCode != 1) throw new ChocoExecutionException(result);

            return new Version(result.Output.First().Replace("Chocolatey v", string.Empty));
        }

        public async Task GetInstalled(ProgressIndication.IProgressIndicator progressIndicator)
        {
            var result = await Execute("upgrade all -r --whatif");
            result.ThrowIfNotSucceeded();

            var tasks = result.Output.Select(t =>
            {
                var tmp = t.Split(Seperator);
                return Task.Run(() => new ChocoItem(Repo.FindPackage(tmp[0]), new SemanticVersion(tmp[1]), new SemanticVersion(tmp[2])));
            }).ToList();

            InstalledPackages = (await Task.WhenAll(tasks)).ToList();

            await Task.WhenAll(InstalledPackages.Select(UpdatePackageVersion));
            InstalledPackages.ForEach(t => t.Actions = ActionFactory.Generate(this, t, progressIndicator));
        }

        public async Task<List<SemanticVersion>> GetVersions(string id)
        {
            return await Task.Run(() => Repo.GetPackages().Where(p => p.Id == id).ToList().Select(p => p.Version).OrderByDescending(p => p.Version).ToList());
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

        public async Task<ChocolateyResult> Execute(string arguments)
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

        public async Task UpdatePackageVersion(ChocoItem package)
        {
            package.Versions = await GetVersions(package.Package.Id);
        }
    }
}
