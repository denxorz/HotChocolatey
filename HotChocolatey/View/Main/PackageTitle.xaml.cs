using AutoDependencyPropertyMarker;
using HotChocolatey.Model;

namespace HotChocolatey.View.Main
{
    public partial class PackageTitle
    {
        [AutoDependencyProperty]
        public Package Package { get; set; }

        public PackageTitle()
        {
            InitializeComponent();
        }
    }
}
