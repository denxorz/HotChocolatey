using HotChocolatey.Model;
using HotChocolatey.Utility;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace HotChocolatey.ViewModel
{
    [Magic]
    public partial class PackageManagerViewModel : INotifyPropertyChanged, ProgressIndication.IProgressIndicator
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<SearchEventArgs> Searched;
        public event EventHandler ScrolledToBottom;

        public ObservableCollection<IFilter> Filters { get; } = new ObservableCollection<IFilter>();

        public IFilter Filter { get; set; }

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

        private ChocoItem selectedPackage;

        public PackageManagerViewModel()
        {
            PropertyChanged += async (s, e) =>
                {
                    if (e.PropertyName == nameof(Filter))
                    {
                        using (new ProgressIndication(this))
                        {
                            await Packages.ApplyFilter(Filter);
                            await Packages.GetMore();
                        }
                    }
                };
        }

        private void InitializeFilter(ChocolateyController controller)
        {
            FilterFactory.BuildFilters(controller, this).ForEach(Filters.Add);
            Filter = Filters.First();
        }

        public void ClearSearchText()
        {
            ClearSearchBox?.Invoke();
        }

        public void Load(ChocolateyController controller)
        {
            InitializeFilter(controller);
            Packages.Items.CollectionChanged += OnPackagesCollectionChanged;
        }

        public void Search(SearchEventArgs e)
        {
            Searched?.Invoke(this, e);
        }

        public void OnScrolledToBottom(object sender, EventArgs e)
        {
            ScrolledToBottom?.Invoke(this, EventArgs.Empty);
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
