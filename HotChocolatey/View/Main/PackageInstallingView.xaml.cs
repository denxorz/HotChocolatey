using AutoDependencyPropertyMarker;
using Denxorz.ObservableCollectionWithAddRange;

namespace HotChocolatey.View.Main
{
    public partial class PackageInstallingView
    {
        [AutoDependencyProperty]
        public ObservableCollectionEx<string> ActionProcessOutput { get; set; }

        public PackageInstallingView()
        {
            InitializeComponent();
        }
    }
}
