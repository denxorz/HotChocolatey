using HotChocolatey.Model;
using HotChocolatey.Utility;
using HotChocolatey.ViewModel.Ginnivan;
using NuGet;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HotChocolatey.ViewModel
{
    [Magic]
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly PackageRepo packageRepo = new PackageRepo();
        private readonly NuGetExecutor nugetExecutor = new NuGetExecutor();
        private readonly ChocoExecutor chocoExecutor;

        private string searchText = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        public Action ClearSearchBox { get; internal set; }

        public ObservableCollectionEx<IPackageDisplayType> Filters { get; } = new ObservableCollectionEx<IPackageDisplayType>();
        public IPackageDisplayType Filter { get; set; }

        public Package SelectedPackage { get; set; }
        public ObservableCollectionEx<Package> Packages { get; } = new ObservableCollectionEx<Package>();
        public bool HasSelectedPackage { get; private set; }

        public IAction SelectedAction { get; set; }
        public SemanticVersion SelectedVersion { get; set; }

        public AwaitableDelegateCommand ActionCommand { get; }
        public AwaitableDelegateCommand RefreshCommand { get; }
        public AwaitableDelegateCommand UpgradeAllCommand { get; }

        public bool IsLogVisible { get; set; }
        public Diagnostics Diagnostics { get; } = new Diagnostics();

        public bool IsInProgress { get; private set; }
        public bool IsUserAllowedToExecuteActions { get; set; } = true;

        public MainWindowViewModel()
        {
            Log.ResetSettings(true, true, true, Diagnostics);
            Log.Info($@"---
Version:{Assembly.GetCallingAssembly().GetName().Version} 
MachineName:{Environment.MachineName} 
OSVersion:{Environment.OSVersion} 
Is64BitOperatingSystem:{Environment.Is64BitOperatingSystem}");

#if !DEBUG
            Application.Current.DispatcherUnhandledException += (s, e) => Log.Error($"DispatcherUnhandledException: {e.Exception}");
#endif

            RefreshCommand = new AwaitableDelegateCommand(ExecuteRefreshCommand);
            UpgradeAllCommand = new AwaitableDelegateCommand(ExecuteUpgradeAllCommand);


            chocoExecutor = new ChocoExecutor(packageRepo, nugetExecutor);
            ActionCommand = new AwaitableDelegateCommand(ExecuteActionCommand);

            PropertyChanged += async (s, e) =>
            {
                if (e.PropertyName == nameof(Filter))
                {
                    await ApplyFilter();
                }

                if (e.PropertyName == nameof(SelectedPackage))
                {
                    HasSelectedPackage = SelectedPackage != null;

                    if (HasSelectedPackage)
                    {
                        await nugetExecutor.GetVersion(SelectedPackage);
                        SelectedAction = SelectedPackage.DefaultAction;
                    }
                }

                if (e.PropertyName == nameof(SelectedAction))
                {
                    SelectedVersion = SelectedAction?.Versions.First();
                }
            };
        }

        public async Task Loaded()
        {
            chocoExecutor.Update();

            Filters.AddRange(PackageDisplayTypeFactory.BuildDisplayTypes(packageRepo, nugetExecutor, chocoExecutor));
            Filter = Filters.First();
        }

        public async Task ClearSearchText()
        {
            await Search(string.Empty);
        }

        public async Task Search(SearchEventArgs e)
        {
            await Search(e.SearchText);
        }

        public async void OnScrolledToBottom(object sender, EventArgs e)
        {
            await GetMorePackages();
        }

        private async Task Search(string searchFor)
        {
            Log.Info($"Searching for: {searchFor}");

            searchText = searchFor;
            await ApplyFilter();
        }

        private async Task ApplyFilter()
        {
            using (new ProgressIndication(() => IsInProgress = true, () => IsInProgress = false))
            {
                Packages.Clear();
                await Filter.ApplySearch(searchText);
                await Filter.Refresh();
                await GetMorePackages();
                if (SelectedPackage == null)
                {
                    SelectedPackage = Packages.FirstOrDefault();
                }
            }
        }

        private async Task GetMorePackages()
        {
            Packages.AddRange(await Filter.GetMore(10));
        }

        private async Task ExecuteActionCommand()
        {
            await SelectedAction.Execute(chocoExecutor, SelectedVersion);
            await chocoExecutor.Update();
        }

        private async Task ExecuteRefreshCommand()
        {
            using (new ProgressIndication(() => IsInProgress = true, () => IsInProgress = false))
            {
                await ClearSearchText();
                await chocoExecutor.Update();
            }
        }

        private async Task ExecuteUpgradeAllCommand()
        {
             using (new ProgressIndication(() => IsUserAllowedToExecuteActions = false, () => IsUserAllowedToExecuteActions = true))
            {
                Filter = PackageDisplayTypeFactory.BuildUpgradeFilter(packageRepo, nugetExecutor, chocoExecutor);
                await ApplyFilter();

                foreach (var package in Packages)
                {
                    await chocoExecutor.Upgrade(package, package.LatestVersion);
                }
            }
        }

        private void RaisePropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
