using HotChocolatey.View;
using HotChocolatey2.ViewModel;
using System.Windows;

namespace HotChocolatey2
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
