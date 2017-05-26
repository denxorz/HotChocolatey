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
            settings.Proxy.Address = LoadSetting<string>("proxy");
            settings.Proxy.User = LoadSetting<string>("proxyUser");
            settings.Proxy.Password = LoadSetting<string>("proxyPassword");

            var chocoTask = new LoadFeaturesChocoTask();
            chocoTask.Execute();

            settings.ChecksumFiles = chocoTask.Features["checksumFiles"];
            settings.AutoUninstaller = chocoTask.Features["autoUninstaller"];
            settings.AllowGlobalConfirmation = chocoTask.Features["allowGlobalConfirmation"];
            settings.FailOnAutoUninstaller = chocoTask.Features["failOnAutoUninstaller"];

            return settings;
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
            new SaveSettingsChocoTask("proxy", settings.Proxy.Address).Execute();
            new SaveSettingsChocoTask("proxyUser", settings.Proxy.User).Execute();
            new SaveSettingsChocoTask("proxyPassword", settings.Proxy.Password).Execute();

            new SaveFeatureChocoTask("checksumFiles", settings.ChecksumFiles).Execute();
            new SaveFeatureChocoTask("autoUninstaller", settings.AutoUninstaller).Execute();
            new SaveFeatureChocoTask("allowGlobalConfirmation", settings.AllowGlobalConfirmation).Execute();
            new SaveFeatureChocoTask("failOnAutoUninstaller", settings.FailOnAutoUninstaller).Execute();
        }
    }
}
