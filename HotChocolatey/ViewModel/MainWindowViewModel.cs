using HotChocolatey.Model;
using HotChocolatey.Utility;
using HotChocolatey.ViewModel.Ginnivan;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace HotChocolatey.ViewModel
{
    [Magic]
    public class MainWindowViewModel : INotifyPropertyChanged, ProgressIndication.IProgressIndicator
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ChocolateyController Controller { get; } = new ChocolateyController();
        public Packages Packages { get; } = new Packages();
        public Diagnostics Diagnostics { get; } = new Diagnostics();

        public bool IsLogVisible { get; set; }
        public bool IsUserAllowedToExecuteActions { get; set; } = true;

        public PackageManagerViewModel PackageManagerViewModel { get; } = new PackageManagerViewModel();

        public AwaitableDelegateCommand RefreshCommand { get; }
        public AwaitableDelegateCommand UpgradeAllCommand { get; }

        bool ProgressIndication.IProgressIndicator.IsInProgress
        {
            set
            {
                IsUserAllowedToExecuteActions = !value;
                if (!value)
                {
                    SynchronizationContext.Current.Post(async state =>
                    {
                        using (new ProgressIndication(PackageManagerViewModel))
                        {
                            await Refresh();
                        }
                    }, null);
                }
            }
        }

        public MainWindowViewModel()
        {
            Log.ResetSettings(true, true, true, Diagnostics);
            Log.Info("---");
            Log.Info($"Version:{Assembly.GetCallingAssembly().GetName().Version} MachineName:{Environment.MachineName} OSVersion:{Environment.OSVersion} Is64BitOperatingSystem:{Environment.Is64BitOperatingSystem}");

            RefreshCommand = new AwaitableDelegateCommand(ExecuteRefreshCommand);
            UpgradeAllCommand = new AwaitableDelegateCommand(ExecuteUpgradeAllCommand);

            Application.Current.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Action(async () => { await Loaded(); }));

            PackageManagerViewModel.Searched += OnSearched;
        }

        public async Task Loaded()
        {
            PackageManagerViewModel.Packages = Packages;
            PackageManagerViewModel.Load();

            using (new ProgressIndication(PackageManagerViewModel))
            {
                try
                {
                    var result = await Controller.GetVersion();
                    Log.Info($"Chocolatey version: {result}");
                }
                catch (Win32Exception ex)
                {
                    Log.Error($"Choco not installed? Message: {ex.Message}");
                    MessageBox.Show("Choco not installed?", "Hot Chocolatey Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                await Refresh();
            }
        }

        private async Task ExecuteRefreshCommand()
        {
            using (new ProgressIndication(PackageManagerViewModel))
            {
                await Refresh();
            }
        }

        private async Task ExecuteUpgradeAllCommand()
        {
            using (new ProgressIndication(this))
            {
                Packages.ApplyFilter(FilterFactory.UpgradeFilter);

                foreach (var package in Packages.Items)
                {
                    if (!await Controller.Upgrade(package))
                    {
                        // TODO : provide some sensible text to the user
                        Log.Error($"Upgrade failed for package:{package.Title}");
                    }
                }
            }
        }

        private async void OnSearched(object sender, SearchEventArgs e)
        {
            Log.Info($"Searching for: {e.SearchText}");

            using (new ProgressIndication(PackageManagerViewModel))
            {
                if (string.IsNullOrWhiteSpace(e.SearchText))
                {
                    await Refresh();
                }
                else
                {
                    Packages.Clear();
                    (await Controller.GetAvailable(e.SearchText, this)).ForEach(Packages.Add);
                }
            }
        }

        private async Task Refresh()
        {
            Log.Info(nameof(Refresh));
            PackageManagerViewModel.ClearSearchText();
            Packages.Clear();
            (await Controller.GetInstalled(this)).ForEach(Packages.Add);
        }

        private void RaisePropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
