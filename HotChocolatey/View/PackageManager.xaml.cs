using HotChocolatey.Utility;
using System;
using System.Windows;
using System.Windows.Controls;

namespace HotChocolatey.View
{
    public partial class PackageManager : UserControl
    {
        private bool done;

        public event EventHandler ScrolledToBottom;

        public PackageManager()
        {
            InitializeComponent();
            PackagesListView.SelectionChanged += PackagesListView_SelectionChanged;
        }

        private void PackagesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (done)
            {
                return;
            }

            if (PackagesListView.ScrollViewer != null)
            {
                PackagesListView.ScrollViewer.ScrollChanged += OnScrollChanged;
                done = true;
            }
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            double offset = PackagesListView.ScrollViewer.VerticalOffset;
            if (offset >= PackagesListView.ScrollViewer.ScrollableHeight)
            {
                ScrolledToBottom?.Invoke(this, EventArgs.Empty);
                PackagesListView.ScrollViewer.ScrollToVerticalOffset(offset);
            }
        }

        private async void OnSearch(object sender, SearchEventArgs e)
        {
            // TODO : Not really neat, but good enough for now
            await (DataContext as ViewModel.MainWindowViewModel).Search(e);
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // TODO : Not really neat, but good enough for now
            (DataContext as ViewModel.MainWindowViewModel).ClearSearchBox = () => SearchTextBox.Clear();
            ScrolledToBottom += (DataContext as ViewModel.MainWindowViewModel).OnScrolledToBottom;
        }
    }
}
