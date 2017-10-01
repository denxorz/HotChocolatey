using System.ComponentModel;
using System.Windows;

namespace HotChocolatey.View.Settings
{
    public partial class SettingsWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // TODO : Not really neat, but good enough for now
            ((ViewModel.SettingsWindowsViewModel)DataContext).Loaded();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            // TODO : Not really neat, but good enough for now
            ((ViewModel.SettingsWindowsViewModel)DataContext).Closing();
        }
    }
}
