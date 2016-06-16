using System.Reflection;
using MahApps.Metro.Controls;

namespace HotChocolatey.View.About
{
    public partial class AboutWindow : MetroWindow
    {
        public AboutWindow()
        {
            InitializeComponent();
            versionTextBlock.Text = Assembly.GetCallingAssembly().GetName().Version.ToString();
        }
    }
}
