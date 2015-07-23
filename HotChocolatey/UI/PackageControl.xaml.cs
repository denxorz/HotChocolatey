using System.Windows;
using System.Windows.Controls;

namespace HotChocolatey.UI
{
    public partial class PackageControl : UserControl
    {
        private PackageControlViewModel ViewModel => DataContext as PackageControlViewModel; // TODO : MVVM ?
        
        public PackageControl()
        {
            InitializeComponent();
        }

        private async void OnActionButtonClick(object sender, RoutedEventArgs e)
        {
            await ViewModel.ActionClicked();
        }
    }
}
