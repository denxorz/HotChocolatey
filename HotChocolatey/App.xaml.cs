using HotChocolatey.View;
using HotChocolatey.ViewModel;
using System.Windows;

namespace HotChocolatey
{
    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            MainWindow view = new MainWindow();
            view.DataContext = new MainWindowViewModel();
            view.Show();
        }
    }
}
