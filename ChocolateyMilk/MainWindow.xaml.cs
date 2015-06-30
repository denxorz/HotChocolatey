using System.Collections.ObjectModel;
using System.Windows;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Input;
using System.Windows.Controls;

namespace ChocolateyMilk
{
    public partial class MainWindow : Window, INotifyPropertyChanged, ProgressIndication.IProgressIndicator
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ChocolateyController Controller { get; } = new ChocolateyController();
        public Packages Packages { get; } = new Packages();
        public ObservableCollection<IFilter> FilterSelections { get; } = new ObservableCollection<IFilter>();
        public IFilter Filter
        {
            get { return selection; }
            set
            {
                if (selection != value)
                {
                    selection = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Filter)));
                }
            }
        }

        public bool IsInProgress
        {
            get { return isInProgress; }
            set
            {
                if (isInProgress != value)
                {
                    isInProgress = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsInProgress)));
                }
            }
        }

        public string StatusText
        {
            get { return statusText; }
            set
            {
                if (statusText != value)
                {
                    statusText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusText)));
                }
            }
        }

        public Visibility LogVisibility
        {
            get { return isLogVisible ? Visibility.Visible : Visibility.Collapsed; }
        }

        public bool IsLogVisible
        {
            get { return isLogVisible; }
            set
            {
                if (isLogVisible != value)
                {
                    isLogVisible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLogVisible)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LogVisibility)));
                }
            }
        }

        private IFilter selection;
        private bool isInProgress;
        private string statusText;
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
                await Controller.GetVersion();
                await Refresh();
            }
        }

        private async Task Refresh()
        {
            StatusText = "Scanning for installed packges";
            (await Controller.GetInstalled()).ForEach(Packages.Add);
            StatusText = "Scanning for updates";
            (await Controller.GetUpgradable()).ForEach(Packages.Add);
            StatusText = "Scanning for new packages";
            (await Controller.GetAvailable()).ForEach(Packages.Add);
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
                await Controller.Install(Packages.MarkedForInstallation);
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
    }
}
