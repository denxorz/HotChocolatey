using System.Windows.Media;
using Bindables;

namespace HotChocolatey.View.About
{
    [DependencyProperty]
    public partial class AboutItem
    {
        public ImageSource IconSource { get; set; }
        public string Title { get; set; }
        public string Website { get; set; }
        public string License { get; set; }

        public AboutItem()
        {
            InitializeComponent();
        }
    }
}
