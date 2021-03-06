﻿using HotChocolatey.Model;
using HotChocolatey.Utility;
using HotChocolatey.ViewModel.Ginnivan;
using NuGet;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Threading;
using Windows.UI.Notifications;
using Denxorz.ObservableCollectionWithAddRange;
using HotChocolatey.Model.Save;
using Settings = HotChocolatey.Properties.Settings;

namespace HotChocolatey.ViewModel
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly PackageRepo packageRepo = new PackageRepo();
        private readonly NuGetExecutor nugetExecutor = new NuGetExecutor();
        private readonly ChocoExecutor chocoExecutor = new ChocoExecutor();

        private string searchText = string.Empty;

        public event EventHandler RequestBringToFront;

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
        public DelegateCommand<bool> SelectAllPackagesCommand { get; }
        public AwaitableDelegateCommand UninstallCheckedCommand { get; }
        public AwaitableDelegateCommand InstallCheckedCommand { get; }
        public AwaitableDelegateCommand UpdateCheckedCommand { get; }


        public bool IsInProgress { get; private set; }
        public bool IsInstalling { get; private set; }
        public bool IsUserAllowedToExecuteActions { get; set; } = true;
        public ObservableCollectionEx<string> ActionProcessOutput { get; } = new ObservableCollectionEx<string>();

        public bool IncludePreReleases { get; set; }

        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        private readonly DispatcherTimer updateCheckTimer = new DispatcherTimer();
        private int numberOfUpdatesLastCheck;

        public MainWindowViewModel()
        {
            Log.ResetSettings(true, true);
            Log.Info($@"---
Version:{Assembly.GetExecutingAssembly().GetName().Version}
MachineName:{Environment.MachineName}
OSVersion:{Environment.OSVersion}
Is64BitOperatingSystem:{Environment.Is64BitOperatingSystem}");

#if !DEBUG
            System.Windows.Application.Current.DispatcherUnhandledException += (s, e) => Log.Error($"DispatcherUnhandledException: {e.Exception}");
#endif

            if (Settings.Default.UpdateRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpdateRequired = false;
                Settings.Default.Save();
            }

            App.SecondInstanceStarted += App_SecondInstanceStarted;

            updateCheckTimer.Interval = TimeSpan.FromHours(2);
            updateCheckTimer.Tick += UpdateCheckTimer_Tick;
            updateCheckTimer.Start();

            RefreshCommand = new AwaitableDelegateCommand(ExecuteRefreshCommandAsync);
            UpgradeAllCommand = new AwaitableDelegateCommand(ExecuteUpgradeAllCommandAsync);
            InstallCommand = new AwaitableDelegateCommand(ExecuteInstallCommandAsync);
            UpdateCommand = new AwaitableDelegateCommand(ExecuteUpdateCommandAsync);
            UninstallCommand = new AwaitableDelegateCommand(ExecuteUninstallCommandAsync);
            ImportCommand = new DelegateCommand(ExecuteImportCommand);
            ExportCommand = new DelegateCommand(ExecuteExportCommand);
            OpenCommandPromptCommand = new DelegateCommand(ExecuteOpenCommandPromptCommand);
            SearchStartedCommand = new AwaitableDelegateCommand(ExecuteSearchStartedCommandAsync);
            SelectAllPackagesCommand = new DelegateCommand<bool>(ExecuteSelectAllPackagesCommand);
            UninstallCheckedCommand = new AwaitableDelegateCommand(ExecuteUninstallCheckedCommand);
            InstallCheckedCommand = new AwaitableDelegateCommand(ExecuteInstallCheckedCommand);
            UpdateCheckedCommand = new AwaitableDelegateCommand(ExecuteUpdateCheckedCommand);

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

        public void Initialized()
        {
            UpdatePacakgeStates();
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
            using (new ProgressIndication(dispatcher, () => IsInProgress = true, () => IsInProgress = false))
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
            var newPackages = Filter.GetMore(10).ToList();

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

        private Task ExecuteInstallCommandAsync()
        {
            return ExecuteActionAsync(new InstallAction(SelectedPackage, SelectedVersion));
        }

        private Task ExecuteUpdateCommandAsync()
        {
            return ExecuteActionAsync(new UpgradeAction(SelectedPackage, SelectedVersion));
        }

        private Task ExecuteUninstallCommandAsync()
        {
            return ExecuteActionAsync(new UninstallAction(SelectedPackage));
        }

        private async Task ExecuteActionAsync(IAction action)
        {
            using (new ProgressIndication(dispatcher, () => IsInstalling = true, () => IsInstalling = false))
            {
                ActionProcessOutput.Clear();
                await Task.Run(() => action.Execute(chocoExecutor, outputLineCallback => dispatcher.Invoke(() => ActionProcessOutput.Add(outputLineCallback))));
                await chocoExecutor.UpdateAsync(packageRepo, nugetExecutor);
            }
        }

        private async Task ExecuteRefreshCommandAsync()
        {
            using (new ProgressIndication(dispatcher, () => IsInProgress = true, () => IsInProgress = false))
            {
                await ClearSearchTextAsync();
                await Task.Run(() => chocoExecutor.UpdateAsync(packageRepo, nugetExecutor));
            }
        }

        private async Task ExecuteUpgradeAllCommandAsync()
        {
            using (new ProgressIndication(dispatcher, () => IsUserAllowedToExecuteActions = false, () => IsUserAllowedToExecuteActions = true))
            {
                ActionProcessOutput.Clear();
                Filter = PackageDisplayTypeFactory.BuildUpgradeFilter(packageRepo, nugetExecutor, chocoExecutor);
                await ApplyFilterAsync();

                var packagesToUpdate = Packages.ToArray(); // The list is copied, because the updated packages can be filtered away.
                chocoExecutor.Upgrade(packagesToUpdate, null, outputLineCallback => ActionProcessOutput.Add(outputLineCallback));
            }

            await ExecuteRefreshCommandAsync();
        }

        private void ExecuteImportCommand()
        {
            using (new ProgressIndication(dispatcher, () => IsInProgress = true, () => IsInProgress = false))
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
            using (new ProgressIndication(dispatcher, () => IsInProgress = true, () => IsInProgress = false))
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

        private void ExecuteSelectAllPackagesCommand(bool isSelected)
        {
            foreach (var package in Packages)
            {
                package.IsChecked = isSelected;
            }
        }

        private Task ExecuteUninstallCheckedCommand()
        {
            return ExecuteActionAsync(new UninstallAction(Packages.Where(p => p.IsChecked).ToArray()));
        }

        private Task ExecuteInstallCheckedCommand()
        {
            return ExecuteActionAsync(new InstallAction(Packages.Where(p => p.IsChecked).ToArray()));
        }

        private Task ExecuteUpdateCheckedCommand()
        {
            return ExecuteActionAsync(new UpgradeAction(Packages.Where(p => p.IsChecked).ToArray()));
        }

        private void UpdateCheckTimer_Tick(object sender, EventArgs e)
        {
            if (!Settings.Default.ShowNotifications) return;

            UpdatePacakgeStates();
        }

        private void UpdatePacakgeStates()
        {
            Task.Run(() => chocoExecutor.UpdateAsync(packageRepo, nugetExecutor))
                .ContinueWith(t => NotifyNumberOfUpdates());
        }

        private void NotifyNumberOfUpdates()
        {
            if (!Settings.Default.ShowNotifications) return;

            var updates = packageRepo.NumberOfUpgradesAvailable;
            if (updates > numberOfUpdatesLastCheck)
            {
                numberOfUpdatesLastCheck = updates;

                var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
                var stringElements = toastXml.GetElementsByTagName("text");

                stringElements[0].AppendChild(toastXml.CreateTextNode($"{packageRepo.NumberOfUpgradesAvailable} updates available"));
                stringElements[1].AppendChild(toastXml.CreateTextNode("Hot Chocolatey found updates. Click here to view the updates."));

                var toast = new ToastNotification(toastXml);
                toast.Activated += ToastActivated;

                ToastNotificationManager.CreateToastNotifier("Denxorz.HotChocolatey").Show(toast);
            }
        }

        private void ToastActivated(ToastNotification sender, object e)
        {
            dispatcher.Invoke(() => RequestBringToFront?.Invoke(this, EventArgs.Empty));
        }

        private void App_SecondInstanceStarted(object sender, EventArgs e)
        {
            dispatcher.Invoke(() => RequestBringToFront?.Invoke(this, EventArgs.Empty));
        }

        public void Closing()
        {
            chocoExecutor.Close();
            Settings.Default.Save();
        }
    }
}
