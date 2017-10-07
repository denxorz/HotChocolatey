using System.Reflection;

namespace HotChocolatey.View.About
{
    public partial class AboutWindow
    {
        public string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public AboutWindow()
        {
            InitializeComponent();
        }
    }
}
