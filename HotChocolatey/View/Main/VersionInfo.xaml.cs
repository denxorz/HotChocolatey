using Bindables;
using HotChocolatey.Model;

namespace HotChocolatey.View.Main
{
    [DependencyProperty]
    public partial class VersionInfo
    {
        public Package Package { get; set; }

        public VersionInfo()
        {
            InitializeComponent();
        }
    }
}
