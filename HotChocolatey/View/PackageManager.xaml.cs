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
            if (done) return;
            if (PackagesListView.ScrollViewer != null)
            {
                PackagesListView.ScrollViewer.ScrollChanged += OnScrollChanged;
                done = true;
            }
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (PackagesListView.ScrollViewer.VerticalOffset == PackagesListView.ScrollViewer.ScrollableHeight)
            {
                ScrolledToBottom?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnSearch(object sender, SearchEventArgs e)
        {
            // TODO : Not really neat, but good enough for now
            (DataContext as ViewModel.PackageManagerViewModel).Search(e);
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // TODO : Not really neat, but good enough for now
            (DataContext as ViewModel.PackageManagerViewModel).ClearSearchBox = () => SearchTextBox.Clear();
            ScrolledToBottom += (DataContext as ViewModel.PackageManagerViewModel).OnScrolledToBottom;
        }
    }
}
