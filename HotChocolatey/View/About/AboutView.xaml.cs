using System.Reflection;

namespace HotChocolatey.View.About
{
    public partial class AboutWindow
    {
        public AboutWindow()
        {
            InitializeComponent();
            versionTextBlock.Text = Assembly.GetCallingAssembly().GetName().Version.ToString();
        }
    }
}
