using System.Windows;
using System.Windows.Controls;

namespace HotChocolatey.View.Main
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            HamburgerMenuControl.SelectedIndex = 0;
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

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // TODO : Not really neat, but good enough for now
            ((ViewModel.MainWindowViewModel)DataContext).Loaded();
        }
    }
}
