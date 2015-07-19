using HotChocolatey.Logic;
using HotChocolatey.Utility;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HotChocolatey.UI
{
    [Magic]
    public partial class PackageManager : UserControl, INotifyPropertyChanged, ProgressIndication.IProgressIndicator
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<SearchEventArgs> Searched;

        public ObservableCollection<IFilter> Filters { get; } = new ObservableCollection<IFilter>();
        public IFilter Filter { get; set; }
        public Packages Packages { get; set; }
        public ChocoItem SelectedPackage { get; set; }
        public bool IsInProgress { get; set; }

        public bool HasSelectedPackage => SelectedPackage != null;

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
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                InitializeFilter();
                DataContext = this;

                Packages.Items.CollectionChanged += OnPackagesCollectionChanged;
            }
        }

        private void OnPackagesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (SelectedPackage == null)
            {
                SelectedPackage = Packages.Items.FirstOrDefault();
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PackageControl.Package = SelectedPackage;
        }

        private void OnSearch(object sender, SearchEventArgs e)
        {
            Searched?.Invoke(this, e);
        }

        private void RaisePropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
