using System.Windows;
using Bindables;
using HotChocolatey.Model;
using HotChocolatey.ViewModel.Ginnivan;
using NuGet;

namespace HotChocolatey.View.Main
{
    [DependencyProperty]
    public partial class PackageNotInstalledControl
    {
        public Package Package { get; set; }
        [DependencyProperty(Options = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
        public SemanticVersion Version { get; set; }
        public AwaitableDelegateCommand InstallCommand { get; set; }

        public PackageNotInstalledControl()
        {
            InitializeComponent();
        }
    }
}
