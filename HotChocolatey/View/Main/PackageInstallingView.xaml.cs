using Bindables;
using Denxorz.ObservableCollectionWithAddRange;

namespace HotChocolatey.View.Main
{
    [DependencyProperty]
    public partial class PackageInstallingView
    {
        public ObservableCollectionEx<string> ActionProcessOutput { get; set; }

        public PackageInstallingView()
        {
            InitializeComponent();
        }
    }
}
