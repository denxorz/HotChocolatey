using System.Windows;

namespace HotChocolatey
{
    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            UI.MainWindow view = new UI.MainWindow();
            view.DataContext = new UI.MainWindowViewModel();
            view.Show();
        }
    }
}
