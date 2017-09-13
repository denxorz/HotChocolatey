using System.Windows;
using AutoDependencyPropertyMarker;
using HotChocolatey.Model;
using HotChocolatey.ViewModel.Ginnivan;
using NuGet;

namespace HotChocolatey.View.Main
{
    public partial class PackageNotInstalledControl
    {
        [AutoDependencyProperty]
        public Package Package { get; set; }

        [AutoDependencyProperty(Options = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
        public SemanticVersion Version { get; set; }

        [AutoDependencyProperty]
        public AwaitableDelegateCommand InstallCommand { get; set; }

        public PackageNotInstalledControl()
        {
            InitializeComponent();
        }
    }
}
