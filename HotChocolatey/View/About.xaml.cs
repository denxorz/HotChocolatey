using System.Reflection;
using MahApps.Metro.Controls;

namespace HotChocolatey.View
{
    public partial class About : MetroWindow
    {
        public About()
        {
            InitializeComponent();
            versionTextBlock.Text = Assembly.GetCallingAssembly().GetName().Version.ToString();
        }
    }
}
