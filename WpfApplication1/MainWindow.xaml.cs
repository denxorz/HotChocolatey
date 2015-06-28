using System.Collections.ObjectModel;
using System.Windows;
using System.ComponentModel;

namespace WpfApplication1
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ChocolateyController Controller { get; } = new ChocolateyController();
        public Packages Packages { get; } = new Packages();
        public ObservableCollection<IFilter> FilterSelections { get; } = new ObservableCollection<IFilter>();
        public IFilter Filter
        {
            get { return selection; }
            set
            {
                if (selection != value)
                {
                    selection = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Filter)));
                }
            }
        }

        private IFilter selection;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            InitializeFilter();

            await Controller.GetVersion();
            (await Controller.GetInstalled()).ForEach(Packages.Add);
            (await Controller.GetAvailable()).ForEach(Packages.Add);
        }

        private void InitializeFilter()
        {
            FilterSelections.Add(new NoFilter());
            FilterSelections.Add(new InstalledFilter());
            FilterSelections.Add(new InstalledUpgradableFilter());
            FilterSelections.Add(new NotInstalledFilter());
            Filter = FilterSelections[0];
        }

        private void OnSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Packages.ApplyFilter(Filter.Filter);
        }
    }
}
