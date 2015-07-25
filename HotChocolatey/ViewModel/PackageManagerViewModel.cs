using HotChocolatey.Model;
using HotChocolatey.Utility;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace HotChocolatey.ViewModel
{
    [Magic]
    public partial class PackageManagerViewModel : INotifyPropertyChanged, ProgressIndication.IProgressIndicator
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<SearchEventArgs> Searched;

        public ObservableCollection<IFilter> Filters { get; } = new ObservableCollection<IFilter>();

        public IFilter Filter
        {
            get { return selectedFilter; }
            set
            {
                selectedFilter = value;
                Packages.ApplyFilter(Filter);
            }
        }

        public ChocoItem SelectedPackage
        {
            get { return selectedPackage; }
            set
            {
                selectedPackage = value;
                PackageControlViewModel.Package = SelectedPackage;
            }
        }

        public Packages Packages { get; set; }
        public bool IsInProgress { get; set; }

        public bool HasSelectedPackage => SelectedPackage != null;

        public PackageControlViewModel PackageControlViewModel { get; } = new PackageControlViewModel();

        public Action ClearSearchBox { get; set; }

        private IFilter selectedFilter;
        private ChocoItem selectedPackage;

        private void InitializeFilter()
        {
            FilterFactory.AvailableFilters.ForEach(Filters.Add);
            Filter = Filters.First();
        }

        public void ClearSearchText()
        {
            ClearSearchBox?.Invoke();
        }

        public void Load()
        {
            InitializeFilter();
            Packages.Items.CollectionChanged += OnPackagesCollectionChanged;
        }

        public void Search(SearchEventArgs e)
        {
            Searched?.Invoke(this, e);
        }

        private void OnPackagesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (SelectedPackage == null)
            {
                SelectedPackage = Packages.Items.FirstOrDefault();
            }
        }

        private void RaisePropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
