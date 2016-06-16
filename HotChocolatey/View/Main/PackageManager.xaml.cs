using System;
using System.Windows;
using System.Windows.Controls;
using HotChocolatey.Utility;

namespace HotChocolatey.View.Main
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

        // False positive, needed for Search component
#pragma warning disable S1144 // Unused private types or members should be removed
        private async void OnSearch(object sender, SearchEventArgs e)
        {
            // TODO : Not really neat, but good enough for now
            await ((ViewModel.MainWindowViewModel)DataContext).Search(e);
        }
#pragma warning restore S1144 // Unused private types or members should be removed

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // TODO : Not really neat, but good enough for now
            ((ViewModel.MainWindowViewModel)DataContext).ClearSearchBox = () => SearchTextBox.Clear();
            ScrolledToBottom += ((ViewModel.MainWindowViewModel)DataContext).OnScrolledToBottom;
        }
    }
}
