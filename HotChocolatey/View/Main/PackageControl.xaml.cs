using System.Windows;
using AutoDependencyPropertyMarker;
using Denxorz.ObservableCollectionWithAddRange;
using HotChocolatey.Model;
using HotChocolatey.ViewModel.Ginnivan;
using NuGet;

namespace HotChocolatey.View.Main
{
    public partial class PackageControl
    {
        [AutoDependencyProperty]
        public Package Package { get; set; }

        [AutoDependencyProperty]
        public bool HasSelectedPackage { get; set; }

        [AutoDependencyProperty(Options = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
        public SemanticVersion Version { get; set; }

        [AutoDependencyProperty]
        public bool IsInstalling { get; set; }

        [AutoDependencyProperty]
        public AwaitableDelegateCommand InstallCommand { get; set; }

        [AutoDependencyProperty]
        public AwaitableDelegateCommand UpdateCommand { get; set; }

        [AutoDependencyProperty]
        public AwaitableDelegateCommand UninstallCommand { get; set; }

        [AutoDependencyProperty]
        public ObservableCollectionEx<string> ActionProcessOutput { get; set; }

        public PackageControl()
        {
            InitializeComponent();
        }
    }
}
