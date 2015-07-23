using HotChocolatey.Utility;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace HotChocolatey.UI
{
    public partial class PackageManager : UserControl
    {
        private ViewModel.PackageManagerViewModel ViewModel => DataContext as ViewModel.PackageManagerViewModel; // TODO : MVVM ?

        public PackageManager()
        {
            InitializeComponent();
        }

        private void OnFilterSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.FilterSelectionChanged();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                ViewModel.Loaded();
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SelectionChanged();
        }

        private void OnSearch(object sender, SearchEventArgs e)
        {
            ViewModel.Search(e);
        }
    }
}
