using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WpfApplication1
{
    public class ChocoItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool Selected { get; set; }

        public string Name { get; private set; }
        public string InstalledVersion { get; private set; }
        public string LatestVersion { get; private set; }

        public bool IsInstalled { get { return !string.IsNullOrEmpty(InstalledVersion); } }
        public bool IsInstalledUpgradable { get { return IsInstalled && !string.IsNullOrEmpty(LatestVersion) && new Version(LatestVersion) > new Version(InstalledVersion); } }

        public static ChocoItem FromInstalledString(string chocoOutput)
        {
            var tmp = chocoOutput.Split('|');
            return new ChocoItem { Name = tmp[0], InstalledVersion = tmp[1], Selected = true };
        }

        public static ChocoItem FromAvailableString(string chocoOutput)
        {
            var tmp = chocoOutput.Split('|');
            return new ChocoItem { Name = tmp[0], LatestVersion = tmp[1] };
        }

        internal void Update(ChocoItem item)
        {
            if (!string.IsNullOrEmpty(item.InstalledVersion) && InstalledVersion != item.InstalledVersion)
            {
                InstalledVersion = item.InstalledVersion;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InstalledVersion)));
            }

            if (!string.IsNullOrEmpty(item.LatestVersion) && LatestVersion != item.LatestVersion)
            {
                LatestVersion = item.LatestVersion;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LatestVersion)));
            }
        }
    }

    public interface IFilter
    {
        Predicate<object> Filter { get; }
    }

    public class NoFilter : IFilter
    {
        public Predicate<object> Filter { get; } = t => true;
        public override string ToString() => "All";
    }

    public class InstalledFilter : IFilter
    {
        public Predicate<object> Filter { get; } = t => (t as ChocoItem).IsInstalled;
        public override string ToString() => "Installed";
    }

    public class InstalledUpgradableFilter : IFilter
    {
        public Predicate<object> Filter { get; } = t => (t as ChocoItem).IsInstalledUpgradable;
        public override string ToString() => "Installed (upgradable)";
    }

    public class NotInstalledFilter : IFilter
    {
        public Predicate<object> Filter { get; } = t => !(t as ChocoItem).IsInstalled;
        public override string ToString() => "Not installed";
    }

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ChocolateyController Controller { get; } = new ChocolateyController();
        public ChocoPackages Packages { get; } = new ChocoPackages();
        public ObservableCollection<IFilter> Selections { get; } = new ObservableCollection<IFilter>();
        public IFilter Selection
        {
            get { return selection; }
            set
            {
                if (selection != value)
                {
                    selection = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Selection)));
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
            Selections.Add(new NoFilter());
            Selections.Add(new InstalledFilter());
            Selections.Add(new InstalledUpgradableFilter());
            Selections.Add(new NotInstalledFilter());
            Selection = Selections[0];

            await Controller.GetVersion();
            (await Controller.GetInstalled()).ForEach(Packages.Add);
            (await Controller.GetAvailable()).ForEach(Packages.Add);
        }

        private void OnSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Packages.ApplyFilter(Selection.Filter);
        }
    }

    public class ChocoPackages
    {
        public ObservableCollection<ChocoItem> Items { get; } = new ObservableCollection<ChocoItem>();
        private readonly Dictionary<string, ChocoItem> packages = new Dictionary<string, ChocoItem>();
        private readonly ICollectionView view;

        public ChocoPackages()
        {
            view = CollectionViewSource.GetDefaultView(Items);
        }

        public void ApplyFilter(Predicate<object> filter)
        {
            view.Filter = filter;
        }

        public void Add(ChocoItem item)
        {
            if (packages.ContainsKey(item.Name))
            {
                packages[item.Name].Update(item);
            }
            else
            {
                packages.Add(item.Name, item);
                Items.Add(item);
            }
        }
    }

    public class ChocolateyController
    {
        public ObservableCollection<string> Output { get; } = new ObservableCollection<string>();

        public async Task<Version> GetVersion()
        {
            return new Version((await Execute(string.Empty))[0].Replace("Chocolatey v", string.Empty));
        }

        public async Task<List<ChocoItem>> GetInstalled()
        {
            return (await Execute("list -l -r")).Select(t => ChocoItem.FromInstalledString(t)).ToList();
        }

        public async Task<List<ChocoItem>> GetAvailable()
        {
            return (await Execute("list -r Atom")).Select(t => ChocoItem.FromAvailableString(t)).ToList();
        }

        private async Task<List<string>> Execute(string arguments)
        {
            Output.Add($"> choco {arguments}");

            Process compiler = new Process();
            compiler.StartInfo.FileName = "choco";
            compiler.StartInfo.Arguments = arguments;
            compiler.StartInfo.UseShellExecute = false;
            compiler.StartInfo.RedirectStandardOutput = true;
            compiler.StartInfo.CreateNoWindow = true;
            compiler.Start();

            string output = await compiler.StandardOutput.ReadToEndAsync();

            compiler.WaitForExit();

            List<string> lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            lines.ForEach(Output.Add);

            return lines;
        }
    }
}
