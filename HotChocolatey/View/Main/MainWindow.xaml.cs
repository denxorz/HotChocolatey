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
            var window = new Settings.SettingsWindow { Owner = this };

            window.ShowDialog();
        }
    }
}
