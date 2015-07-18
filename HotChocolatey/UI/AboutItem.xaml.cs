using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HotChocolatey.UI
{
    public partial class AboutItem : UserControl
    {
        public static DependencyProperty IconSourceProperty =
            DependencyProperty.Register(
                nameof(IconSource),
                typeof(ImageSource),
                typeof(AboutItem),
                new PropertyMetadata(new BitmapImage(new Uri("/HotChocolatey;component/Images/chocolateyicon.gif", UriKind.Relative))));

        public static DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(AboutItem),
                new PropertyMetadata("Title"));

        public static DependencyProperty WebsiteProperty =
            DependencyProperty.Register(
                nameof(Website),
                typeof(string),
                typeof(AboutItem),
                new PropertyMetadata("http://denxorz.nl"));

        public static DependencyProperty LicenseProperty =
            DependencyProperty.Register(
                nameof(License),
                typeof(string),
                typeof(AboutItem),
                new PropertyMetadata("http://denxorz.nl"));

        public ImageSource IconSource
        {
            get { return (ImageSource)GetValue(IconSourceProperty); }
            set { SetValue(IconSourceProperty, value); }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public string Website
        {
            get { return (string)GetValue(WebsiteProperty); }
            set { SetValue(WebsiteProperty, value); }
        }

        public string License
        {
            get { return (string)GetValue(LicenseProperty); }
            set { SetValue(LicenseProperty, value); }
        }

        public AboutItem()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void OnLinkClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Website);
        }
    }
}
