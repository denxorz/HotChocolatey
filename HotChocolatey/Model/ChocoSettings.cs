using System.ComponentModel;

namespace HotChocolatey.Model
{
    public class ChocoSettings
    {
        [DisplayName("Cache location")]
        public string CacheLocation { get; set; }

        [DisplayName("Command execution timeout in seconds")]
        public int CommandExecutionTimeoutSeconds { get; set; }

        [DisplayName("Contains legacy package installs")]
        public bool ContainsLegacyPackageInstalls { get; set; }

        public int WebRequestTimeoutSeconds { get; set; }
        public string Proxy { get; set; }
        public string ProxyUser { get; set; }
        public string ProxyPassword { get; set; }
        public string ProxyBypassList { get; set; }
        public bool ProxyBypassOnLocal { get; set; }
    }
}
