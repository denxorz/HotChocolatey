using MahApps.Metro.Controls;

namespace HotChocolatey.View
{
    public partial class SettingsWindow : MetroWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // TODO : Not really neat, but good enough for now
            ((ViewModel.SettingsWindowsViewModel)DataContext).Closing();
        }
    }
}
