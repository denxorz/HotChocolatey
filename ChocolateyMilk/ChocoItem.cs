
using System;
using System.ComponentModel;
using NuGet;

namespace ChocolateyMilk
{
    [Magic]
    public class ChocoItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        public IPackage Package { get; }

        public string Name => Package.Id;
        public string InstalledVersion { get; private set; }
        public string LatestVersion { get; private set; }
        public bool IsInstalledUpgradable { get; private set; }
        public bool IsMarkedForInstallation { get; set; }
        public bool IsMarkedForUpgrade { get; set; }
        public bool IsMarkedForUninstall { get; set; }

        public bool IsInstalled => InstalledVersion != null;

        public ChocoItem(IPackage package)
        {
            Package = package;
        }

        public static ChocoItem FromInstalledString(IPackage package, string version)
        {
            return new ChocoItem(package) { InstalledVersion = version };
        }

        public static ChocoItem FromPackage(IPackage package)
        {
            return new ChocoItem(package);
        }

        public static ChocoItem FromUpdatableString(IPackage package, string installedVersion, string latestVersion)
        {
            return new ChocoItem(package) { InstalledVersion = installedVersion, LatestVersion = latestVersion, IsInstalledUpgradable = installedVersion != latestVersion };
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

        private void RaisePropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

