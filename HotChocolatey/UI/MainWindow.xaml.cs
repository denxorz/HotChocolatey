using HotChocolatey.Logic;
using HotChocolatey.Utility;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HotChocolatey.UI
{
    [Magic]
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ChocolateyController Controller { get; } = new ChocolateyController();
        public Packages Packages { get; } = new Packages();
        public Diagnostics Diagnostics { get; } = new Diagnostics();

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

        private void OnToolBarLoaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }
            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
            {
                mainPanelBorder.Margin = new Thickness();
            }
        }

        private async Task Refresh()
        {
            Log.Info(nameof(Refresh));
            Packages.Clear();
            (await Controller.GetInstalled()).ForEach(Packages.Add);
        }

        private async void OnRefreshClick(object sender, RoutedEventArgs e)
        {
            using (new ProgressIndication(PackageManager))
            {
                await Refresh();
            }
        }

        private async void OnSearchClick(object sender, SearchEventArgs e)
        {
            Log.Info($"Searching for: {e.SearchText}");

            using (new ProgressIndication(PackageManager))
            {
                if (string.IsNullOrWhiteSpace(e.SearchText))
                {
                    await Refresh();
                }
                else
                {
                    Packages.Clear();
                    (await Controller.GetAvailable(e.SearchText)).ForEach(Packages.Add);
                }
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

        private void OnAboutButtonClick(object sender, RoutedEventArgs e)
        {
            var about = new About();
            about.ShowDialog();
        }
    }
}
