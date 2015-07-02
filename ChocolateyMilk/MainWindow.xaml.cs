using System.Collections.ObjectModel;
using System.Windows;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Input;
using System.Windows.Controls;
using System;

namespace ChocolateyMilk
{
    [Magic]
    public partial class MainWindow : Window, INotifyPropertyChanged, ProgressIndication.IProgressIndicator
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ChocolateyController Controller { get; } = new ChocolateyController();
        public Packages Packages { get; } = new Packages();
        public ObservableCollection<IFilter> FilterSelections { get; } = new ObservableCollection<IFilter>();

        public Visibility LogVisibility => isLogVisible ? Visibility.Visible : Visibility.Collapsed;

        public IFilter Filter { get; set; }
        public bool IsInProgress { get; set; }
        public string StatusText { get; set; }

        public bool IsLogVisible
        {
            get { return isLogVisible; }
            set
            {
                if (isLogVisible != value)
                {
                    isLogVisible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LogVisibility)));
                }
            }
        }

        private bool isLogVisible;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            InitializeFilter();

            using (new ProgressIndication(this))
            {
                StatusText = "Getting version info";

                try
                {
                    await Controller.GetVersion();
                }
                catch (Win32Exception)
                {
                    MessageBox.Show("Choco not installed?", "ChocolateyMilk Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                await Refresh();
            }
        }

        private async Task Refresh()
        {
            StatusText = "Scanning for installed packges";
            (await Controller.GetInstalled()).ForEach(Packages.Add);
            StatusText = "Scanning for updates";
            (await Controller.GetUpgradable()).ForEach(Packages.Add);

            // TODO : decide if this can be done without paging?
            //StatusText = "Scanning for new packages";
            //(await Controller.GetAvailable()).ForEach(Packages.Add);
        }

        private void InitializeFilter()
        {
            FilterFactory.AvailableFilters.ForEach(FilterSelections.Add);
            Filter = FilterSelections[0];
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Packages.ApplyFilter(Filter.Filter);
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

        private void OnShowLoggingClick(object sender, RoutedEventArgs e)
        {
            IsLogVisible = !IsLogVisible;
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

        private void RaisePropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
