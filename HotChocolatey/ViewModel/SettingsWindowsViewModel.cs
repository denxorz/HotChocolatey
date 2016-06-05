using System.Threading.Tasks;
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
    }
}
