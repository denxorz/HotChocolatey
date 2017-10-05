using NuGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolatey.Model.ChocoTask;

namespace HotChocolatey.Model
{
    public class ChocoExecutor
    {
        public List<Package> LocalPackages { get; } = new List<Package>();
        public bool IncludePreReleases { get; set; }

        public async Task UpdateAsync(PackageRepo repo, NuGetExecutor nuGetExecutor)
        {
            LocalPackages.Clear();
            repo.ClearLocalVersions();
            new UpdateLocalChocoTask(IncludePreReleases, repo, LocalPackages.Add).Execute();
            var updateNuGetInfoAsync = UpdateNuGetInfoAsync(nuGetExecutor);
            new UpdateOutdatedFlagsChocoTask(repo).Execute();
            await updateNuGetInfoAsync;
        }

        public void Install(Package package, SemanticVersion specificVersion, Action<string> outputLineCallback)
        {
            new InstallChocoTask(outputLineCallback, IncludePreReleases, package, specificVersion).Execute();
        }

        public void Uninstall(Package package, Action<string> outputLineCallback)
        {
            new UninstallChocoTask(outputLineCallback, package).Execute();
        }

        public void Upgrade(Package package, SemanticVersion specificVersion, Action<string> outputLineCallback)
        {
            new UpgradeChocoTask(outputLineCallback, IncludePreReleases, package, specificVersion).Execute();
        }

        private async Task UpdateNuGetInfoAsync(NuGetExecutor nuGetExecutor)
        {
            await Task.WhenAll(LocalPackages.Select(t => Task.Run(() => nuGetExecutor.Update(t))));
        }

        public ChocoSettings LoadSettings()
        {
            var settings = new ChocoSettings();
            settings.CacheLocation = LoadSetting<string>("cacheLocation");
            settings.CommandExecutionTimeoutSeconds = LoadSetting<int>("commandExecutionTimeoutSeconds");
            settings.ContainsLegacyPackageInstalls = LoadSetting<bool>("containsLegacyPackageInstalls");
            settings.Proxy = LoadSetting<string>("proxy");
            settings.ProxyUser = LoadSetting<string>("proxyUser");
            settings.ProxyPassword = LoadSetting<string>("proxyPassword");
            settings.ProxyBypassList = LoadSetting<string>("proxyBypassList");
            settings.ProxyBypassOnLocal = LoadSetting<bool>("proxyBypassOnLocal");
            settings.WebRequestTimeoutSeconds = LoadSetting<int>("webRequestTimeoutSeconds");
            return settings;
        }

        public List<ChocoFeature> LoadFeatures()
        {
            var chocoTask = new LoadFeaturesChocoTask();
            chocoTask.Execute();
            return chocoTask.Features;
        }

        private T LoadSetting<T>(string name)
        {
            var chocoTask = new LoadSettingChocoTask(name);
            chocoTask.Execute();
            return (T)Convert.ChangeType(chocoTask.Setting, typeof(T));
        }

        public void SaveSettings(ChocoSettings settings)
        {
            new SaveSettingsChocoTask("cacheLocation", settings.CacheLocation).Execute();
            new SaveSettingsChocoTask("commandExecutionTimeoutSeconds", settings.CommandExecutionTimeoutSeconds).Execute();
            new SaveSettingsChocoTask("containsLegacyPackageInstalls", settings.ContainsLegacyPackageInstalls).Execute();
            new SaveSettingsChocoTask("proxy", settings.Proxy).Execute();
            new SaveSettingsChocoTask("proxyUser", settings.ProxyUser).Execute();
            new SaveSettingsChocoTask("proxyPassword", settings.ProxyPassword).Execute();
            new SaveSettingsChocoTask("proxyBypassList", settings.ProxyBypassList).Execute();
            new SaveSettingsChocoTask("proxyBypassOnLocal", settings.ProxyBypassOnLocal).Execute();
            new SaveSettingsChocoTask("webRequestTimeoutSeconds", settings.WebRequestTimeoutSeconds).Execute();
        }

        public void SaveFeature(ChocoFeature feature)
        {
            new SaveFeatureChocoTask(feature.Name, feature.IsEnabled).Execute();
        }
    }
}
