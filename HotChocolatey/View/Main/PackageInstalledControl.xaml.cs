using System.Windows;
using Bindables;
using HotChocolatey.Model;
using HotChocolatey.ViewModel.Ginnivan;
using NuGet;

namespace HotChocolatey.View.Main
{
    [DependencyProperty]
    public partial class PackageInstalledControl
    {
        public Package Package { get; set; }
        [DependencyProperty(Options = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
        public SemanticVersion Version { get; set; }
        public AwaitableDelegateCommand UpdateCommand { get; set; }
        public AwaitableDelegateCommand UninstallCommand { get; set; }

        public PackageInstalledControl()
        {
            InitializeComponent();
        }
    }
}
