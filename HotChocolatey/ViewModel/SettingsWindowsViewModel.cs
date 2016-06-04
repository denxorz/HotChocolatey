using System.ComponentModel;
using System.Threading.Tasks;
using HotChocolatey.Model;

namespace HotChocolatey.ViewModel
{
    [Magic]
    public class SettingsWindowsViewModel : INotifyPropertyChanged
    {
        public ChocoSettings Settings { get; private set; } = new ChocoSettings();
        public bool IsLoading { get; private set; }
        private readonly ChocoExecutor chocoExecutor = new ChocoExecutor();

        public event PropertyChangedEventHandler PropertyChanged;
        
        public async Task Loaded()
        {
            IsLoading = true;
            Settings = await chocoExecutor.LoadSettings();
            IsLoading = false;
        }

        public async Task Closing()
        {
            await chocoExecutor.SaveSettings(Settings);
        }

#pragma warning disable S1144 // Unused private types or members should be removed

        private void RaisePropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

#pragma warning restore S1144 // Unused private types or members should be removed
    }
}
