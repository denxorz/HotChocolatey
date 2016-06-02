using HotChocolatey.Model;

namespace HotChocolatey.ViewModel
{
    [Magic]
    class SettingsWindowsViewModel
    {
        public ChocoSettings Settings { get; set; }
        private readonly ChocoExecutor chocoExecutor;


        public SettingsWindowsViewModel()
        {
            Settings = new ChocoSettings();
        }

        public void Closing()
        {
            //Save();
        }
    }
}
