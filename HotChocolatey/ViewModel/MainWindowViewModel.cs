using HotChocolatey.Model;
using HotChocolatey.Utility;
using HotChocolatey.ViewModel.Ginnivan;
using NuGet;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Threading;
using PropertyChanged;

namespace HotChocolatey.ViewModel
{
    [ImplementPropertyChanged]
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly PackageRepo packageRepo = new PackageRepo();
        private readonly NuGetExecutor nugetExecutor = new NuGetExecutor();
        private readonly ChocoExecutor chocoExecutor = new ChocoExecutor();

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
        public DelegateCommand OpenCommandPromptCommand { get; }
        public DelegateCommand SearchStartedCommand { get; }

        public bool IsInProgress { get; private set; }
        public bool IsInstalling { get; private set; }
        public bool IsUserAllowedToExecuteActions { get; set; } = true;
        public ObservableCollectionEx<string> ActionProcessOutput { get; } = new ObservableCollectionEx<string>();

        public bool IncludePreReleases { get; set; }

        public MainWindowViewModel()
        {
            Log.ResetSettings(true, true);
            ChocoCommunication.SetDispatcher(Dispatcher.CurrentDispatcher);
            Log.Info($@"---
Version:{Assembly.GetCallingAssembly().GetName().Version}
MachineName:{Environment.MachineName}
OSVersion:{Environment.OSVersion}
Is64BitOperatingSystem:{Environment.Is64BitOperatingSystem}");

#if !DEBUG
            System.Windows.Application.Current.DispatcherUnhandledException += (s, e) => Log.Error($"DispatcherUnhandledException: {e.Exception}");
#endif

            RefreshCommand = new AwaitableDelegateCommand(ExecuteRefreshCommand);
            UpgradeAllCommand = new AwaitableDelegateCommand(ExecuteUpgradeAllCommand);
            ActionCommand = new AwaitableDelegateCommand(ExecuteActionCommand);
            OpenCommandPromptCommand = new DelegateCommand(ExecuteOpenCommandPromptCommand);
            SearchStartedCommand = new DelegateCommand(ExecuteSearchStartedCommand);

            PropertyChanged += async (s, e) =>
            {
                if (e.PropertyName == nameof(IncludePreReleases))
                {
                    chocoExecutor.IncludePreReleases = IncludePreReleases;
                }

                if (e.PropertyName == nameof(Filter) || e.PropertyName == nameof(IncludePreReleases))
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

        public void Loaded()
        {
            Task.Run(() => chocoExecutor.Update(packageRepo, nugetExecutor));

            Filters.AddRange(PackageDisplayTypeFactory.BuildDisplayTypes(packageRepo, nugetExecutor, chocoExecutor));
            Filter = Filters.First();
        }

        public async Task ClearSearchText()
        {
            await Search(string.Empty);
        }

        public async void OnScrolledToBottom(object sender, EventArgs e)
        {
            await GetMorePackages();
        }

        public async Task Search(string searchFor)
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
            Task actionTask;
            using (new ProgressIndication(() => IsInstalling = true, () => IsInstalling = false))
            {
                ActionProcessOutput.Clear();
                await SelectedAction.Execute(chocoExecutor, SelectedVersion, outputLineCallback => ActionProcessOutput.Add(outputLineCallback));
                actionTask = Task.Run(() => chocoExecutor.Update(packageRepo, nugetExecutor));
            }

            await actionTask;
        }

        private async Task ExecuteRefreshCommand()
        {
            using (new ProgressIndication(() => IsInProgress = true, () => IsInProgress = false))
            {
                await ClearSearchText();
                await Task.Run(() => chocoExecutor.Update(packageRepo, nugetExecutor));
            }
        }

        private async Task ExecuteUpgradeAllCommand()
        {
            using (new ProgressIndication(() => IsUserAllowedToExecuteActions = false, () => IsUserAllowedToExecuteActions = true))
            {
                ActionProcessOutput.Clear();
                Filter = PackageDisplayTypeFactory.BuildUpgradeFilter(packageRepo, nugetExecutor, chocoExecutor);
                await ApplyFilter();

                foreach (var package in Packages)
                {
                    await chocoExecutor.Upgrade(package, package.LatestVersion, outputLineCallback => ActionProcessOutput.Add(outputLineCallback));
                }
            }

            await ExecuteRefreshCommand();
        }


        private void ExecuteOpenCommandPromptCommand()
        {
            Process.Start("cmd");
        }

        private async void ExecuteSearchStartedCommand()
        {
            await ClearSearchText();
        }
    }
}
