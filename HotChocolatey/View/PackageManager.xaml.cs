using HotChocolatey.Utility;
using System.Windows.Controls;

namespace HotChocolatey.View
{
    public partial class PackageManager : UserControl
    {
        public PackageManager()
        {
            InitializeComponent();
        }

        private void OnSearch(object sender, SearchEventArgs e)
        {
            // TODO : Not really neat, but good enough for now
            (DataContext as ViewModel.PackageManagerViewModel).Search(e);
        }

        private void UserControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            // TODO : Not really neat, but good enough for now
            (DataContext as ViewModel.PackageManagerViewModel).ClearSearchBox = () => SearchTextBox.Clear();
        }
    }
}
