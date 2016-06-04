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

        public async Task Update(PackageRepo repo, NuGetExecutor nuGetExecutor)
        {
            LocalPackages.Clear();
            repo.ClearLocalVersions();
            await new UpdateLocalChocoTask(IncludePreReleases, repo, LocalPackages.Add).Execute();
            await new UpdateOutdatedFlagsChocoTask(repo).Execute();
            await UpdateNuGetInfo(nuGetExecutor);
        }

        public async Task Install(Package package, SemanticVersion specificVersion, Action<string> outputLineCallback)
        {
            await new InstallChocoTask(outputLineCallback, IncludePreReleases, package, specificVersion).Execute();
        }

        public async Task Uninstall(Package package, Action<string> outputLineCallback)
        {
            await new UninstallChocoTask(outputLineCallback, package).Execute();
        }

        public async Task Upgrade(Package package, SemanticVersion specificVersion, Action<string> outputLineCallback)
        {
            await new UpgradeChocoTask(outputLineCallback, IncludePreReleases, package, specificVersion).Execute();
        }

        private async Task UpdateNuGetInfo(NuGetExecutor nuGetExecutor)
        {
            await Task.WhenAll(LocalPackages.Select(t => Task.Run(() => nuGetExecutor.Update(t))));
        }

        public async Task<ChocoSettings> LoadSettings()
        {
            var settings = new ChocoSettings();
            settings.CacheLocation = await LoadSetting<string>("cacheLocation");
            settings.CommandExecutionTimeoutSeconds = await LoadSetting<int>("commandExecutionTimeoutSeconds");
            settings.ContainsLegacyPackageInstalls = await LoadSetting<bool>("containsLegacyPackageInstalls");
            settings.Proxy.Address = await LoadSetting<string>("proxy");
            settings.Proxy.User = await LoadSetting<string>("proxyUser");
            settings.Proxy.Password = await LoadSetting<string>("proxyPassword");

            var chocoTask = new LoadFeaturesChocoTask();
            await chocoTask.Execute();

            settings.ChecksumFiles = chocoTask.Features["checksumFiles"];
            settings.AutoUninstaller = chocoTask.Features["autoUninstaller"];
            settings.AllowGlobalConfirmation = chocoTask.Features["allowGlobalConfirmation"];
            settings.FailOnAutoUninstaller = chocoTask.Features["failOnAutoUninstaller"];

            return settings;
        }

        private async Task<T> LoadSetting<T>(string name)
        {
            var chocoTask = new LoadSettingChocoTask(name);
            await chocoTask.Execute();
            return (T)Convert.ChangeType(chocoTask.Setting, typeof(T));
        }

        public async Task SaveSettings(ChocoSettings settings)
        {
            await new SaveSettingsChocoTask("cacheLocation", settings.CacheLocation).Execute();
            await new SaveSettingsChocoTask("commandExecutionTimeoutSeconds", settings.CommandExecutionTimeoutSeconds).Execute();
            await new SaveSettingsChocoTask("containsLegacyPackageInstalls", settings.ContainsLegacyPackageInstalls).Execute();
            await new SaveSettingsChocoTask("proxy", settings.Proxy.Address).Execute();
            await new SaveSettingsChocoTask("proxyUser", settings.Proxy.User).Execute();
            await new SaveSettingsChocoTask("proxyPassword", settings.Proxy.Password).Execute();

            await new SaveFeatureChocoTask("checksumFiles", settings.ChecksumFiles).Execute();
            await new SaveFeatureChocoTask("autoUninstaller", settings.AutoUninstaller).Execute();
            await new SaveFeatureChocoTask("allowGlobalConfirmation", settings.AllowGlobalConfirmation).Execute();
            await new SaveFeatureChocoTask("failOnAutoUninstaller", settings.FailOnAutoUninstaller).Execute();
        }

        private async Task<T> SaveSetting<T>(string name)
        {
            var chocoTask = new LoadSettingChocoTask(name);
            await chocoTask.Execute();
            return (T)Convert.ChangeType(chocoTask.Setting, typeof(T));
        }
    }
}
