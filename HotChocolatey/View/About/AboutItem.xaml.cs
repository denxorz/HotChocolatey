using System.Windows.Media;
using AutoDependencyPropertyMarker;

namespace HotChocolatey.View.About
{
    public partial class AboutItem
    {
        [AutoDependencyProperty]
        public ImageSource IconSource { get; set; }

        [AutoDependencyProperty]
        public string Title { get; set; }

        [AutoDependencyProperty]
        public string Website { get; set; }

        [AutoDependencyProperty]
        public string License { get; set; }

        public AboutItem()
        {
            InitializeComponent();
        }
    }
}
