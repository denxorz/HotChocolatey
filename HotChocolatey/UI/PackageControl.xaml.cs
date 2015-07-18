using HotChocolatey.Logic;
using NuGet;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace HotChocolatey.UI
{
    [Magic]
    public partial class PackageControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ChocoItem package;

        public ChocoItem Package
        {
            get { return package; }
            set
            {
                package = value;

                if (package != null)
                {
                    PackageAction = package.Actions.First();
                    PackageVersion = PackageAction.Versions.First();
                }
                HasPackage = package != null;

                Raise();
            }
        }

        public IAction PackageAction { get; set; }
        public SemanticVersion PackageVersion { get; set; }
        public bool HasPackage { get; private set; }

        public PackageControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void RaisePropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        [MethodImpl(MethodImplOptions.NoInlining)] // to preserve method call 
        protected static void Raise() { }

        private async void OnActionButtonClick(object sender, RoutedEventArgs e)
        {
            await PackageAction.Execute(PackageVersion);
        }
    }
}
