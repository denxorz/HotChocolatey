using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using AutoDependencyPropertyMarker;

namespace HotChocolatey.View
{
    public partial class LinkTextBlock : UserControl
    {
        [AutoDependencyProperty]
        public string NavigationUrl { get; set; }

        public LinkTextBlock()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void OnLinkClicked(object sender, RoutedEventArgs e)
        {
            Process.Start(NavigationUrl);
        }
    }
}
