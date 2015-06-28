using System;
using System.ComponentModel;

namespace WpfApplication1
{
    public class ChocoItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool Selected { get; set; }

        public string Name { get; private set; }
        public string InstalledVersion { get; private set; }
        public string LatestVersion { get; private set; }

        public bool IsInstalled { get { return !string.IsNullOrEmpty(InstalledVersion); } }
        public bool IsInstalledUpgradable { get { return IsInstalled && !string.IsNullOrEmpty(LatestVersion) && new Version(LatestVersion) > new Version(InstalledVersion); } }

        public static ChocoItem FromInstalledString(string chocoOutput)
        {
            var tmp = chocoOutput.Split(ChocolateyController.Seperator);
            return new ChocoItem { Name = tmp[0], InstalledVersion = tmp[1], Selected = true };
        }

        public static ChocoItem FromAvailableString(string chocoOutput)
        {
            var tmp = chocoOutput.Split(ChocolateyController.Seperator);
            return new ChocoItem { Name = tmp[0], LatestVersion = tmp[1] };
        }

        internal void Update(ChocoItem item)
        {
            if (!string.IsNullOrEmpty(item.InstalledVersion) && InstalledVersion != item.InstalledVersion)
            {
                InstalledVersion = item.InstalledVersion;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InstalledVersion)));
            }

            if (!string.IsNullOrEmpty(item.LatestVersion) && LatestVersion != item.LatestVersion)
            {
                LatestVersion = item.LatestVersion;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LatestVersion)));
            }
        }
    }
}
