﻿using HotChocolatey.Model;
using HotChocolatey.ViewModel.Ginnivan;
using NuGet;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace HotChocolatey.ViewModel
{
    [Magic]
    public partial class PackageControlViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly ProgressIndication.IProgressIndicator installIndicator;
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

        public AwaitableDelegateCommand ActionCommand { get; }

        public PackageControlViewModel(ProgressIndication.IProgressIndicator installIndicator)
        {
            this.installIndicator = installIndicator;
            ActionCommand = new AwaitableDelegateCommand(ExecuteActionCommand);
        }

        private async Task ExecuteActionCommand()
        {
            using (new ProgressIndication(installIndicator))
            {
                await PackageAction.Execute(PackageVersion);
            }
        }

        private void RaisePropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        [MethodImpl(MethodImplOptions.NoInlining)] // to preserve method call 
        protected static void Raise() { }
    }
}
