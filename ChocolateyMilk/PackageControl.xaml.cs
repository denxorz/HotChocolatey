using System.Windows.Controls;

namespace ChocolateyMilk
{
    [Magic]
    public partial class PackageControl : UserControl
    {
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
    }
}
