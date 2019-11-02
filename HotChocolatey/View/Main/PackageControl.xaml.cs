using System.Windows;
using Bindables;
using Denxorz.ObservableCollectionWithAddRange;
using HotChocolatey.Model;
using HotChocolatey.ViewModel.Ginnivan;
using NuGet;

namespace HotChocolatey.View.Main
{
    [DependencyProperty]
    public partial class PackageControl
    {
        public Package Package { get; set; }
        public bool HasSelectedPackage { get; set; }

        [DependencyProperty(Options = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
        public SemanticVersion Version { get; set; }
        public bool IsInstalling { get; set; }
        public AwaitableDelegateCommand InstallCommand { get; set; }
        public AwaitableDelegateCommand UpdateCommand { get; set; }
        public AwaitableDelegateCommand UninstallCommand { get; set; }
        public ObservableCollectionEx<string> ActionProcessOutput { get; set; }

        public PackageControl()
        {
            InitializeComponent();
        }
    }
}
