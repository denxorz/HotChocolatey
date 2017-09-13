using AutoDependencyPropertyMarker;
using HotChocolatey.Model;

namespace HotChocolatey.View.Main
{
    public partial class PackageInfoView
    {
        [AutoDependencyProperty]
        public Package Package { get; set; }

        public PackageInfoView()
        {
            InitializeComponent();
        }
    }
}
