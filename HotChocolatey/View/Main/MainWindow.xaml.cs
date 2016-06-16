using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using HotChocolatey.View.About;
using MahApps.Metro.Controls;

namespace HotChocolatey.View.Main
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            ((INotifyCollectionChanged)loggingListBox.Items).CollectionChanged += OnLoggingListViewCollectionChanged;
        }

        private void OnToolBarLoaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = (ToolBar)sender;
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

        private void OnLoggingListViewCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                loggingListBox.ScrollIntoView(e.NewItems[0]);
            }
        }

        private void OnAboutButtonClick(object sender, RoutedEventArgs e)
        {
            var about = new AboutWindow { Owner = this };
            about.ShowDialog();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // TODO : Not really neat, but good enough for now
            ((ViewModel.MainWindowViewModel)DataContext).Loaded();
        }

        private void OnSettingsButtonClick(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new Settings.SettingsWindow { Owner = this };

            settingsWindow.ShowDialog();
        }
    }
}
