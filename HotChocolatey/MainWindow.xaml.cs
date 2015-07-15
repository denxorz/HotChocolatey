using System.Windows;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using System.Reflection;
using System.Collections.Specialized;

namespace HotChocolatey
{
    [Magic]
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ChocolateyController Controller { get; } = new ChocolateyController();
        public Packages Packages { get; } = new Packages();
        public Diagnostics Diagnostics { get; } = new Diagnostics();

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

            using (new ProgressIndication(PackageManager))
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

        private async Task Refresh()
        {
            Log.Info(nameof(Refresh));

            (await Controller.GetInstalled()).ForEach(Packages.Add);
            (await Controller.GetUpgradable()).ForEach(Packages.Add);

            // TODO : decide if this can be done without paging?
            //(await Controller.GetAvailable()).ForEach(Packages.Add);
        }

        private async void OnRefreshClick(object sender, RoutedEventArgs e)
        {
            using (new ProgressIndication(PackageManager))
            {
                Packages.Clear();
                await Refresh();
            }
        }

        private async void OnSearchClick(object sender, RoutedEventArgs e)
        {
            Log.Info($"Searching for: {SearchText}");

            using (new ProgressIndication(PackageManager))
            {
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
