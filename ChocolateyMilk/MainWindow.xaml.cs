using System.Collections.ObjectModel;
using System.Windows;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using System.Reflection;
using System.Collections.Specialized;

namespace ChocolateyMilk
{
    [Magic]
    public partial class MainWindow : Window, INotifyPropertyChanged, ProgressIndication.IProgressIndicator
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ChocolateyController Controller { get; } = new ChocolateyController();
        public Packages Packages { get; } = new Packages();
        public Diagnostics Diagnostics { get; } = new Diagnostics();

        public bool IsInProgress { get; set; }
        public string StatusText { get; set; }
        public string SearchText { get; set; }
        public bool IsLogVisible { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

        // TODO :    ((INotifyCollectionChanged)loggingListBox.Items).CollectionChanged += OnLoggingListViewCollectionChanged;

            Log.ResetSettings(true, true, true, Diagnostics);
            Log.Info("---");
            Log.Info($"Version:{Assembly.GetCallingAssembly().GetName().Version} MachineName:{Environment.MachineName} OSVersion:{Environment.OSVersion} Is64BitOperatingSystem:{Environment.Is64BitOperatingSystem}");
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            PackageManager.Packages = Packages;

            using (new ProgressIndication(this))
            {
                StatusText = "Getting version info";

                try
                {
                    var result = await Controller.GetVersion();
                    Log.Info($"Chocolatey version: {result}");
                }
                catch (Win32Exception ex)
                {
                    Log.Error($"Choco not installed? Message: {ex.Message}");
                    MessageBox.Show("Choco not installed?", "ChocolateyMilk Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                await Refresh();
            }
        }

        private async Task Refresh()
        {
            Log.Info(nameof(Refresh));

            StatusText = "Scanning for installed packges";
            (await Controller.GetInstalled()).ForEach(Packages.Add);
            StatusText = "Scanning for updates";
          // TODO :  (await Controller.GetUpgradable()).ForEach(Packages.Add);

            // TODO : decide if this can be done without paging?
            //StatusText = "Scanning for new packages";
            //(await Controller.GetAvailable()).ForEach(Packages.Add);
        }

        private async void OnRefreshClick(object sender, RoutedEventArgs e)
        {
            using (new ProgressIndication(this))
            {
                Packages.Clear();
                await Refresh();
            }
        }

        private void OnMarkAllUpgradesClick(object sender, RoutedEventArgs e)
        {
            Packages.Items.Where(t => t.IsInstalledUpgradable).ToList().ForEach(t => t.IsMarkedForUpgrade = true);
        }

        private async void OnApplyClick(object sender, RoutedEventArgs e)
        {
            using (new ProgressIndication(this))
            {
                StatusText = "Installing new packages";
                bool installResult = await Controller.Install(Packages.MarkedForInstallation);

                StatusText = "Upgrading packages";
                bool upgradingResult = await Controller.Upgrade(Packages.MarkedForUpgrade);

                StatusText = "Removing packages";
                bool uninstallResult = await Controller.Uninstall(Packages.MarkedForUninstall);

                if (!installResult || !upgradingResult || !uninstallResult)
                {
                    MessageBox.Show($"Apply failed.{Environment.NewLine}Installing:{installResult}{Environment.NewLine}Upgrading:{upgradingResult}{Environment.NewLine}Removing:{uninstallResult}",
                        "ChocolateyMilk Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                await Refresh();
            }
        }

        private async void OnSearchClick(object sender, RoutedEventArgs e)
        {
            Log.Info($"Searching for: {SearchText}");

            using (new ProgressIndication(this))
            {
                StatusText = "Searching for packages";
                (await Controller.GetAvailable(SearchText)).ForEach(Packages.Add);
            }
        }

        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;
            if (!cell.IsEditing)
            {
                // enables editing on single click
                if (!cell.IsFocused)
                    cell.Focus();
                if (!cell.IsSelected)
                    cell.IsSelected = true;
            }
        }

        private void OnLoggingListViewCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
             // TODO :   loggingListBox.ScrollIntoView(e.NewItems[0]);
            }
        }

        private void RaisePropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
