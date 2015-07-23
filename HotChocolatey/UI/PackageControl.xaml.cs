using System.Windows;
using System.Windows.Controls;

namespace HotChocolatey.UI
{
    public partial class PackageControl : UserControl
    {
        private ViewModel.PackageControlViewModel ViewModel => DataContext as ViewModel.PackageControlViewModel; // TODO : MVVM ?
        
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
