using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace HotChocolatey.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ((INotifyCollectionChanged)loggingListBox.Items).CollectionChanged += OnLoggingListViewCollectionChanged;
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

        private void OnLoggingListViewCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                loggingListBox.ScrollIntoView(e.NewItems[0]);
            }
        }

        private void OnAboutButtonClick(object sender, RoutedEventArgs e)
        {
            var about = new About();
            about.Owner = this;
            about.ShowDialog();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            // TODO : Not really neat, but good enough for now
            await (DataContext as ViewModel.MainWindowViewModel).Loaded();
        }
    }
}
