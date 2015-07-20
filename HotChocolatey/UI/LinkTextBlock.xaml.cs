using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace HotChocolatey.UI
{
    public partial class LinkTextBlock : UserControl
    {
        public static DependencyProperty NavigationUrlProperty =
        DependencyProperty.Register(
            nameof(NavigationUrl),
            typeof(string),
            typeof(LinkTextBlock),
            new PropertyMetadata("https://gitlab.com/jjb3/HotChocolatey"));

        public string NavigationUrl
        {
            get { return (string)GetValue(NavigationUrlProperty); }
            set { SetValue(NavigationUrlProperty, value); }
        }

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
