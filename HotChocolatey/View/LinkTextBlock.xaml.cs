using Bindables;
using System.Diagnostics;
using System.Windows;

namespace HotChocolatey.View
{
    [DependencyProperty]
    public partial class LinkTextBlock
    {
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
