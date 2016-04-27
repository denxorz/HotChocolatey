using System.Reflection;
using System.Windows;

namespace HotChocolatey.View
{
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
            versionTextBlock.Text = Assembly.GetCallingAssembly().GetName().Version.ToString();
        }
    }
}
