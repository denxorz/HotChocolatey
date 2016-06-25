using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // TODO : Not really neat, but good enough for now
            ScrolledToBottom += ((ViewModel.MainWindowViewModel)DataContext).OnScrolledToBottom;
        }

        private async void SearchTextbox_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // TODO : Not really neat, but good enough for now
                await ((ViewModel.MainWindowViewModel)DataContext).Search(((TextBox)sender).Text);
            }
        }
    }
}
