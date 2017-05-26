using HotChocolatey.Model;
using PropertyChanged;

namespace HotChocolatey.ViewModel
{
    [ImplementPropertyChanged]
    public class SettingsWindowsViewModel
    {
        public ChocoSettings Settings { get; private set; } = new ChocoSettings();
        public bool IsLoading { get; private set; }
        private readonly ChocoExecutor chocoExecutor = new ChocoExecutor();

        public void Loaded()
        {
            IsLoading = true;
            Settings = chocoExecutor.LoadSettings();
            IsLoading = false;
        }

        public void Closing()
        {
            chocoExecutor.SaveSettings(Settings);
        }
    }
}
