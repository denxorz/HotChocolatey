using HotChocolatey.Logic;
using HotChocolatey.Utility;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace HotChocolatey.UI
{
    [Magic]
    public partial class PackageManagerViewModel : INotifyPropertyChanged, ProgressIndication.IProgressIndicator
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<SearchEventArgs> Searched;

        public ObservableCollection<IFilter> Filters { get; } = new ObservableCollection<IFilter>();
        public IFilter Filter { get; set; }
        public Packages Packages { get; set; }
        public ChocoItem SelectedPackage { get; set; }
        public bool IsInProgress { get; set; }

        public bool HasSelectedPackage => SelectedPackage != null;

        public PackageControlViewModel PackageControlViewModel { get; } = new PackageControlViewModel();

        private void InitializeFilter()
        {
            FilterFactory.AvailableFilters.ForEach(Filters.Add);
            Filter = Filters.First();
        }

        public void ClearSearchText()
        {
            // TODO SearchTextBox.Clear();
        }

        public void FilterSelectionChanged()
        {
            Packages.ApplyFilter(Filter);
        }

        public void Loaded()
        {
            InitializeFilter();
            Packages.Items.CollectionChanged += OnPackagesCollectionChanged;
        }

        public void SelectionChanged()
        {
            PackageControlViewModel.Package = SelectedPackage;
        }

        public void Search(SearchEventArgs e)
        {
            Searched?.Invoke(this, e);
        }

        private void OnPackagesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (SelectedPackage == null)
            {
                SelectedPackage = Packages.Items.FirstOrDefault();
            }
        }

        private void RaisePropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
