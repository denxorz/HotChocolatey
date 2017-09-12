using System.Diagnostics;
using System.Windows;
using AutoDependencyPropertyMarker;

namespace HotChocolatey.View
{
    public partial class LinkTextBlock
    {
        [AutoDependencyProperty]
        public string NavigationUrl { get; set; }

        public LinkTextBlock()
        {
            InitializeComponent();
        }

        private void OnLinkClicked(object sender, RoutedEventArgs e)
        {
            Process.Start(NavigationUrl);
        }
    }
}
