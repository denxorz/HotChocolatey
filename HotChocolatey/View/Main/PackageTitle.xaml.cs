using Bindables;
using HotChocolatey.Model;

namespace HotChocolatey.View.Main
{
    [DependencyProperty]
    public partial class PackageTitle
    {
        public Package Package { get; set; }

        public PackageTitle()
        {
            InitializeComponent();
        }
    }
}
