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
        public ObservableCollectionWithAddRange<ChocoFeature> ChocolateyFeatures { get; } = new ObservableCollectionWithAddRange<ChocoFeature>();
        public ObservableCollectionWithAddRange<ChocoFeature> HotChocolateyFeatures { get; } = new ObservableCollectionWithAddRange<ChocoFeature>();

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CancelCommand { get; }

        private readonly ChocoExecutor chocoExecutor = new ChocoExecutor();
        private readonly HotChocolateyFeatures hotChocolateyFeatures = new HotChocolateyFeatures();
        private bool[] origionalChocolateyFeatures;
        private bool[] origionalHotChocolateyFeatures;

        public SettingsWindowsViewModel()
        {
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Load);
        }

        public void Load()
        {
            Settings = chocoExecutor.LoadSettings();

            var chocolateyFeatures = chocoExecutor.LoadFeatures();
            origionalChocolateyFeatures = chocolateyFeatures.Select(f => f.IsEnabled).ToArray();
            ChocolateyFeatures.ClearAndAddRange(chocolateyFeatures);

            var loadedHotChocolateyFeatures = hotChocolateyFeatures.LoadFeatures();
            origionalHotChocolateyFeatures = loadedHotChocolateyFeatures.Select(f => f.IsEnabled).ToArray();
            HotChocolateyFeatures.ClearAndAddRange(loadedHotChocolateyFeatures);
        }

        public void Save()
        {
            chocoExecutor.SaveSettings(Settings);

            if (ChocolateyFeatures.Count != origionalChocolateyFeatures.Length) throw new NotSupportedException();

            for (int i = 0; i < ChocolateyFeatures.Count; i++)
            {
                if (origionalChocolateyFeatures[i] != ChocolateyFeatures[i].IsEnabled)
                {
                    chocoExecutor.SaveFeature(ChocolateyFeatures[i]);
                }
            }
            origionalChocolateyFeatures = ChocolateyFeatures.Select(f => f.IsEnabled).ToArray();

            for (int i = 0; i < HotChocolateyFeatures.Count; i++)
            {
                if (origionalHotChocolateyFeatures[i] != HotChocolateyFeatures[i].IsEnabled)
                {
                    hotChocolateyFeatures.SaveFeature(HotChocolateyFeatures[i]);
                }
            }
            origionalHotChocolateyFeatures = HotChocolateyFeatures.Select(f => f.IsEnabled).ToArray();
        }
    }
}
