using System.ComponentModel;
using System.Windows;
using MahApps.Metro.Controls;

namespace HotChocolatey.View.Settings
{
    public partial class SettingsWindow : MetroWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            // TODO : Not really neat, but good enough for now
            await ((ViewModel.SettingsWindowsViewModel)DataContext).LoadedAsync();
        }

        private async void OnClosing(object sender, CancelEventArgs e)
        {
            // TODO : Not really neat, but good enough for now
            await ((ViewModel.SettingsWindowsViewModel)DataContext).ClosingAsync();
        }
    }
}
