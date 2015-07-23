using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace HotChocolatey.UI
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel ViewModel => DataContext as MainWindowViewModel; // TODO : MVVM ?

        public MainWindow()
        {
            InitializeComponent();
            ((INotifyCollectionChanged)loggingListBox.Items).CollectionChanged += OnLoggingListViewCollectionChanged;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                await ViewModel.Loaded();
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

        private async void OnRefreshClick(object sender, RoutedEventArgs e)
        {
            await ViewModel.RefreshClicked();
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
            ViewModel.AboutClicked(this);
        }

        private async void UpgradeAllClick(object sender, RoutedEventArgs e)
        {
            await ViewModel.UpgradeAllClicked();
        }
    }
}
