using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HotChocolatey.View
{
    public partial class AboutItem : UserControl
    {
        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register(
                nameof(IconSource),
                typeof(ImageSource),
                typeof(AboutItem),
                new PropertyMetadata(new BitmapImage(new Uri("/HotChocolatey;component/Images/Logos/Hot Chocolate-100.png", UriKind.Relative))));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(AboutItem),
                new PropertyMetadata("Title"));

        public static readonly DependencyProperty WebsiteProperty =
            DependencyProperty.Register(
                nameof(Website),
                typeof(string),
                typeof(AboutItem),
                new PropertyMetadata("https://gitlab.com/jjb3/HotChocolatey"));

        public static readonly DependencyProperty LicenseProperty =
            DependencyProperty.Register(
                nameof(License),
                typeof(string),
                typeof(AboutItem),
                new PropertyMetadata("https://gitlab.com/jjb3/HotChocolatey"));

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
    }
}
