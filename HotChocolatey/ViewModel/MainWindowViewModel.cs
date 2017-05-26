using HotChocolatey.Model;
using HotChocolatey.Utility;
using HotChocolatey.ViewModel.Ginnivan;
using NuGet;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HotChocolatey.Model.Save;
using PropertyChanged;

namespace HotChocolatey.ViewModel
{
    [ImplementPropertyChanged]
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly PackageRepo packageRepo = new PackageRepo();
        private readonly NuGetExecutor nugetExecutor = new NuGetExecutor();
        private readonly ChocoExecutor chocoExecutor = new ChocoExecutor();

        private string searchText = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        public Action ClearSearchBox { get; internal set; }

        public ObservableCollectionEx<IPackageDisplayType> Filters { get; } = new ObservableCollectionEx<IPackageDisplayType>();
        public IPackageDisplayType Filter { get; set; }

        public Package SelectedPackage { get; set; } = Package.Empty;
        public ObservableCollectionEx<Package> Packages { get; } = new ObservableCollectionEx<Package>();
        public bool HasSelectedPackage { get; private set; }

        public SemanticVersion SelectedVersion { get; set; }

        public AwaitableDelegateCommand InstallCommand { get; }
        public AwaitableDelegateCommand UpdateCommand { get; }
        public AwaitableDelegateCommand UninstallCommand { get; }
        public AwaitableDelegateCommand RefreshCommand { get; }
        public AwaitableDelegateCommand UpgradeAllCommand { get; }
        public DelegateCommand ImportCommand { get; }
        public DelegateCommand ExportCommand { get; }
        public DelegateCommand OpenCommandPromptCommand { get; }
        public AwaitableDelegateCommand SearchStartedCommand { get; }

        public bool IsInProgress { get; private set; }
        public bool IsInstalling { get; private set; }
        public bool IsUserAllowedToExecuteActions { get; set; } = true;
        public ObservableCollectionEx<string> ActionProcessOutput { get; } = new ObservableCollectionEx<string>();

        public bool IncludePreReleases { get; set; }

        public MainWindowViewModel()
        {
            Log.ResetSettings(true, true);
            Log.Info($@"---
Version:{Assembly.GetCallingAssembly().GetName().Version}
MachineName:{Environment.MachineName}
OSVersion:{Environment.OSVersion}
Is64BitOperatingSystem:{Environment.Is64BitOperatingSystem}");

#if !DEBUG
            System.Windows.Application.Current.DispatcherUnhandledException += (s, e) => Log.Error($"DispatcherUnhandledException: {e.Exception}");
#endif

            RefreshCommand = new AwaitableDelegateCommand(ExecuteRefreshCommandAsync);
            UpgradeAllCommand = new AwaitableDelegateCommand(ExecuteUpgradeAllCommandAsync);
            InstallCommand = new AwaitableDelegateCommand(ExecuteInstallCommandAsync);
            UpdateCommand = new AwaitableDelegateCommand(ExecuteUpdateCommandAsync);
            UninstallCommand = new AwaitableDelegateCommand(ExecuteUninstallCommandAsync);
            ImportCommand = new DelegateCommand(ExecuteImportCommand);
            ExportCommand = new DelegateCommand(ExecuteExportCommand);
            OpenCommandPromptCommand = new DelegateCommand(ExecuteOpenCommandPromptCommand);
            SearchStartedCommand = new AwaitableDelegateCommand(ExecuteSearchStartedCommandAsync);

            PropertyChanged += async (s, e) =>
            {
                if (e.PropertyName == nameof(IncludePreReleases))
                {
                    chocoExecutor.IncludePreReleases = IncludePreReleases;
                }

                if (e.PropertyName == nameof(Filter) || e.PropertyName == nameof(IncludePreReleases))
                {
                    await ApplyFilterAsync();
                }

                if (e.PropertyName == nameof(SelectedPackage))
                {
                    HasSelectedPackage = SelectedPackage != Package.Empty && SelectedPackage != null;

                    if (HasSelectedPackage)
                    {
                        SelectedVersion = SelectedPackage.Versions.First();
                    }
                }
            };
        }

        public void Loaded()
        {
            Task.Run(() => chocoExecutor.UpdateAsync(packageRepo, nugetExecutor));

            Filters.AddRange(PackageDisplayTypeFactory.BuildDisplayTypes(packageRepo, nugetExecutor, chocoExecutor));
            Filter = Filters.First();
        }

        public async Task ClearSearchTextAsync()
        {
            await SearchAsync(string.Empty);
        }

        public async void OnScrolledToBottom(object sender, EventArgs e)
        {
            await GetMorePackagesAsync();
        }

        public async Task SearchAsync(string searchFor)
        {
            Log.Info($"Searching for: {searchFor}");

            searchText = searchFor;
            await ApplyFilterAsync();
        }

        private async Task ApplyFilterAsync()
        {
            using (new ProgressIndication(() => IsInProgress = true, () => IsInProgress = false))
            {
                Packages.Clear();
                await Filter.ApplySearchAsync(searchText);
                await GetMorePackagesAsync();
                if (SelectedPackage == Package.Empty || SelectedPackage == null)
                {
                    SelectedPackage = Packages.FirstOrDefault();
                }
            }
        }

        private async Task GetMorePackagesAsync()
        {
            var newPackages = (await Filter.GetMoreAsync(10)).ToList();

            if (newPackages.Any())
            {
                await nugetExecutor.GetVersionAsync(newPackages.First());
                foreach (var item in newPackages.Skip(1))
                {
                    nugetExecutor.GetVersionAsync(item);
                }
            }

            Packages.AddRange(newPackages);
        }

        private async Task ExecuteInstallCommandAsync()
        {
            await ExecuteActionAsync(new InstallAction(SelectedPackage));
        }

        private async Task ExecuteUpdateCommandAsync()
        {
            await ExecuteActionAsync(new UpgradeAction(SelectedPackage));
        }

        private async Task ExecuteUninstallCommandAsync()
        {
            await ExecuteActionAsync(new UninstallAction(SelectedPackage));
        }

        private async Task ExecuteActionAsync(IAction action)
        {
            Task actionTask;
            using (new ProgressIndication(() => IsInstalling = true, () => IsInstalling = false))
            {
                ActionProcessOutput.Clear();
                action.Execute(chocoExecutor, SelectedVersion, outputLineCallback => ActionProcessOutput.Add(outputLineCallback));
                actionTask = chocoExecutor.UpdateAsync(packageRepo, nugetExecutor);
            }

            await actionTask;
        }

        private async Task ExecuteRefreshCommandAsync()
        {
            using (new ProgressIndication(() => IsInProgress = true, () => IsInProgress = false))
            {
                await ClearSearchTextAsync();
                await Task.Run(() => chocoExecutor.UpdateAsync(packageRepo, nugetExecutor));
            }
        }

        private async Task ExecuteUpgradeAllCommandAsync()
        {
            using (new ProgressIndication(() => IsUserAllowedToExecuteActions = false, () => IsUserAllowedToExecuteActions = true))
            {
                ActionProcessOutput.Clear();
                Filter = PackageDisplayTypeFactory.BuildUpgradeFilter(packageRepo, nugetExecutor, chocoExecutor);
                await ApplyFilterAsync();

                var packagesToUpdate = Packages.ToList(); // The list is copied, because the updated packages can be filtered away.
                foreach (var package in packagesToUpdate)
                {
                    chocoExecutor.Upgrade(package, package.LatestVersion, outputLineCallback => ActionProcessOutput.Add(outputLineCallback));
                }
            }

            await ExecuteRefreshCommandAsync();
        }

        private void ExecuteImportCommand()
        {
            using (new ProgressIndication(() => IsInProgress = true, () => IsInProgress = false))
            {
                Microsoft.Win32.OpenFileDialog openDialog = new Microsoft.Win32.OpenFileDialog
                {
                    FileName = "HotChocolateyStorage",
                    DefaultExt = ".sqlite",
                    Filter = "Hot Chocolatey Storage (.sqlite)|*.sqlite|All Files|*.*"
                };

                if (openDialog.ShowDialog() == true)
                {
                    foreach (var workStation in new ExportExecutor().Import(openDialog.FileName))
                    {
                        var importFilter = new ImportedPackageDisplayType(
                            workStation.Name,
                            packageRepo,
                            nugetExecutor,
                            chocoExecutor,
                            workStation.InstalledPackages.Select(p => p.Id));
                        Filters.Add(importFilter);
                    }
                }

                SelectedPackage = Package.Empty;
                Filter = Filters.Last();
            }
        }

        private void ExecuteExportCommand()
        {
            using (new ProgressIndication(() => IsInProgress = true, () => IsInProgress = false))
            {
                Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = "HotChocolateyStorage",
                    DefaultExt = ".sqlite",
                    Filter = "Hot Chocolatey Storage (.sqlite)|*.sqlite|All Files|*.*"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    new ExportExecutor().Export(saveDialog.FileName, chocoExecutor.LocalPackages);
                }
            }
        }

        private void ExecuteOpenCommandPromptCommand()
        {
            Process.Start("cmd");
        }

        private async Task ExecuteSearchStartedCommandAsync()
        {
            await ClearSearchTextAsync();
        }
    }
}
