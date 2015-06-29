﻿using System.Collections.ObjectModel;
using System.Windows;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Linq;

namespace ChocolateyMilk
{
    public partial class MainWindow : Window, INotifyPropertyChanged
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

            IsInProgress = true;
            StatusText = "Getting version info";
            await Controller.GetVersion();
            await Refresh();
            StatusText = "Ready";
            IsInProgress = false;
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

        private void OnSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Packages.ApplyFilter(Filter.Filter);
        }

        private async void OnRefreshClick(object sender, RoutedEventArgs e)
        {
            await Refresh();
        }

        private void OnMarkAllUpgradesClick(object sender, RoutedEventArgs e)
        {
            Packages.Items.Where(t => t.IsInstalledUpgradable).ToList().ForEach(t => t.IsMarkedForInstallation = true);
        }

        private void OnShowLoggingClick(object sender, RoutedEventArgs e)
        {
            IsLogVisible = !IsLogVisible;
        }
    }
}
