using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace HotChocolatey.Model
{
    [DisplayName("Chocolatey Settings")]
    class ChocoSettings
    {
        public ChocoSettings()
        {
            Proxy = new ProxySettings();
        }

        [DisplayName("Cache location")]
        public string CacheLocation { get; set; }

        [DisplayName("Command execution timeout in seconds")]
        public int CommandExecutionTimeoutSeconds { get; set; }

        [DisplayName("Contains legacy package installs")]
        public bool ContainsLegacyPackageInstalls { get; set; }

        [DisplayName("Checksum files")]
        public bool ChecksumFiles { get; set; }

        [DisplayName("Auto uninstaller")]
        public bool AutoUninstaller { get; set; }

        [DisplayName("Allow global confirmation")]
        public bool AllowGlobalConfirmation { get; set; }

        [DisplayName("Fail on auto uninstaller")]
        public bool FailOnAutoUninstaller { get; set; }

        [ExpandableObject]
        public ProxySettings Proxy { get; set; }
    }

    class ProxySettings
    {
        public string Address { get; set; }
        public string User { get; set; }
        public string Password { get; set; }

        public override string ToString()
        {
            return $"{Address} - {User}";
        }
    }
}
