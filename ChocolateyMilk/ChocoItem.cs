
namespace ChocolateyMilk
{
    public class ChocoItem : NotifyPropertyChangedImplementation
    {
        public string Name { get; private set; }
        public string InstalledVersion { get; private set; }
        public string LatestVersion { get; private set; }
        public bool IsInstalledUpgradable { get; private set; }
        public bool IsMarkedForInstallation { get; set; }
        public bool IsMarkedForUpgrade { get; set; }

        public bool IsInstalled => InstalledVersion != null;

        public static ChocoItem FromInstalledString(string chocoOutput)
        {
            var tmp = chocoOutput.Split(ChocolateyController.Seperator);
            return new ChocoItem { Name = tmp[0], InstalledVersion = tmp[1] };
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

