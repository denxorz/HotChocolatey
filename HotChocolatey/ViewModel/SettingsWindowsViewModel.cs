using System;
using System.Linq;
using Denxorz.ObservableCollectionWithAddRange;
using HotChocolatey.Model;
using HotChocolatey.ViewModel.Ginnivan;
using PropertyChanged;

namespace HotChocolatey.ViewModel
{
    [ImplementPropertyChanged]
    public class SettingsWindowsViewModel
    {
        public ChocoSettings Settings { get; private set; } = new ChocoSettings();
        public ObservableCollectionWithAddRange<ChocoFeature> Features { get; } = new ObservableCollectionWithAddRange<ChocoFeature>();

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CancelCommand { get; }

        private readonly ChocoExecutor chocoExecutor = new ChocoExecutor();
        private bool[] origionalFeatures;

        public SettingsWindowsViewModel()
        {
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Load);
        }

        public void Load()
        {
            Settings = chocoExecutor.LoadSettings();

            var features = chocoExecutor.LoadFeatures();
            origionalFeatures = features.Select(f => f.IsEnabled).ToArray();
            Features.ClearAndAddRange(features);
        }

        public void Save()
        {
            chocoExecutor.SaveSettings(Settings);

            if (Features.Count != origionalFeatures.Length) throw new NotSupportedException();

            for (int i = 0; i < Features.Count; i++)
            {
                if (origionalFeatures[i] != Features[i].IsEnabled)
                {
                    chocoExecutor.SaveFeature(Features[i]);
                }
            }
        }
    }
}
