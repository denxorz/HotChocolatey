using System.Windows;
using AutoDependencyPropertyMarker;
using HotChocolatey.Model;
using HotChocolatey.ViewModel.Ginnivan;
using NuGet;

namespace HotChocolatey.View.Main
{
    public partial class PackageInstalledControl
    {
        [AutoDependencyProperty]
        public Package Package { get; set; }

        [AutoDependencyProperty(Options = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
        public SemanticVersion Version { get; set; }

        [AutoDependencyProperty]
        public AwaitableDelegateCommand UpdateCommand { get; set; }

        [AutoDependencyProperty]
        public AwaitableDelegateCommand UninstallCommand { get; set; }

        public PackageInstalledControl()
        {
            InitializeComponent();
        }
    }
}
