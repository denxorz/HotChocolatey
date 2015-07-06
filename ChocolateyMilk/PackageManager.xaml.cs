using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ChocolateyMilk
{
    [Magic]
    public partial class PackageManager : UserControl
    {
        public ObservableCollection<IFilter> Filters { get; } = new ObservableCollection<IFilter>();
        public IFilter Filter { get; set; }
        public Packages Packages { get; set; }
        public ChocoItem SelectedPackage { get; set; }

        public PackageManager()
        {
            InitializeComponent();
        }

        private void InitializeFilter()
        {
            FilterFactory.AvailableFilters.ForEach(Filters.Add);
            Filter = Filters.First();
        }

        private void OnFilterSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Packages.ApplyFilter(Filter);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            InitializeFilter();
            DataContext = this;

            Packages.Items.CollectionChanged += OnPackagesCollectionChanged;
        }

        private void OnPackagesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (SelectedPackage == null)
            {
                SelectedPackage = Packages.Items.First();
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PackageControl.Package = SelectedPackage;
        }
    }
}
