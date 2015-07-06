using System.ComponentModel;
using System.Windows.Controls;

namespace ChocolateyMilk
{
    [Magic]
    public partial class PackageControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ChocoItem Package
        {
            get { return package; }
            set
            {
                package = value;
                DataContext = package;
            }
        }

        private ChocoItem package;

        public PackageControl()
        {
            InitializeComponent();
        }

        private void RaisePropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
