using System.ComponentModel;
using System.Windows.Controls;

namespace HotChocolatey
{
    [Magic]
    public partial class PackageControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ChocoItem Package { get; set; }

        public PackageControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void RaisePropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
