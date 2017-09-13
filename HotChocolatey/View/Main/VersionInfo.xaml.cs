using AutoDependencyPropertyMarker;
using HotChocolatey.Model;

namespace HotChocolatey.View.Main
{
    public partial class VersionInfo
    {
        [AutoDependencyProperty]
        public Package Package { get; set; }

        public VersionInfo()
        {
            InitializeComponent();
        }
    }
}
