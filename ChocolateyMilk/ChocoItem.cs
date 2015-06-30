using System.ComponentModel;

namespace ChocolateyMilk
{
    public class ChocoItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string installedVersion;
        private string latestVersion;
        private bool isInstalledUpgradable;
        private bool isMarkedForInstallation;
        private bool isMarkedForUpgrade;

        public bool Selected { get; set; }
        public string Name { get; private set; }

        public string InstalledVersion
        {
            get { return installedVersion; }
            private set
            {
                if (installedVersion != value)
                {
                    installedVersion = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InstalledVersion)));
                }
            }
        }

        public string LatestVersion
        {
            get { return latestVersion; }
            private set
            {
                if (latestVersion != value)
                {
                    latestVersion = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LatestVersion)));
                }
            }
        }

        public bool IsInstalled { get { return InstalledVersion != null; } }

        public bool IsInstalledUpgradable
        {
            get { return isInstalledUpgradable; }
            private set
            {
                if (isInstalledUpgradable != value)
                {
                    isInstalledUpgradable = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsInstalledUpgradable)));
                }
            }
        }

        public bool IsMarkedForInstallation
        {
            get { return isMarkedForInstallation; }
            set
            {
                if (isMarkedForInstallation != value)
                {
                    isMarkedForInstallation = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMarkedForInstallation)));
                }
            }
        }

        public bool IsMarkedForUpgrade
        {
            get { return isMarkedForUpgrade; }
            set
            {
                if (isMarkedForUpgrade != value)
                {
                    isMarkedForUpgrade = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMarkedForUpgrade)));
                }
            }
        }

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

        public static ChocoItem FromUpdatableString(string chocoOutput)
        {
            var tmp = chocoOutput.Split(ChocolateyController.Seperator);
            return new ChocoItem { Name = tmp[0], InstalledVersion = tmp[1], LatestVersion = tmp[2], IsInstalledUpgradable = tmp[1] != tmp[2] };
        }

        internal void Update(ChocoItem item)
        {
            if (item.InstalledVersion != null && InstalledVersion != item.InstalledVersion)
            {
                InstalledVersion = item.InstalledVersion;
            }

            if (item.LatestVersion != null && LatestVersion != item.LatestVersion)
            {
                LatestVersion = item.LatestVersion;
            }

            IsInstalledUpgradable = item.IsInstalledUpgradable;
        }
    }
}
