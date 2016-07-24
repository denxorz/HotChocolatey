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
            await new UpdateLocalChocoTask(IncludePreReleases, repo, LocalPackages.Add).ExecuteAsync();
            await new UpdateOutdatedFlagsChocoTask(repo).ExecuteAsync();
            await UpdateNuGetInfoAsync(nuGetExecutor);
        }

        public async Task InstallAsync(Package package, SemanticVersion specificVersion, Action<string> outputLineCallback)
        {
            await new InstallChocoTask(outputLineCallback, IncludePreReleases, package, specificVersion).ExecuteAsync();
        }

        public async Task UninstallAsync(Package package, Action<string> outputLineCallback)
        {
            await new UninstallChocoTask(outputLineCallback, package).ExecuteAsync();
        }

        public async Task UpgradeAsync(Package package, SemanticVersion specificVersion, Action<string> outputLineCallback)
        {
            await new UpgradeChocoTask(outputLineCallback, IncludePreReleases, package, specificVersion).ExecuteAsync();
        }

        private async Task UpdateNuGetInfoAsync(NuGetExecutor nuGetExecutor)
        {
            await Task.WhenAll(LocalPackages.Select(t => Task.Run(() => nuGetExecutor.Update(t))));
        }

        public async Task<ChocoSettings> LoadSettingsAsync()
        {
            var settings = new ChocoSettings();
            settings.CacheLocation = await LoadSettingAsync<string>("cacheLocation");
            settings.CommandExecutionTimeoutSeconds = await LoadSettingAsync<int>("commandExecutionTimeoutSeconds");
            settings.ContainsLegacyPackageInstalls = await LoadSettingAsync<bool>("containsLegacyPackageInstalls");
            settings.Proxy.Address = await LoadSettingAsync<string>("proxy");
            settings.Proxy.User = await LoadSettingAsync<string>("proxyUser");
            settings.Proxy.Password = await LoadSettingAsync<string>("proxyPassword");

            var chocoTask = new LoadFeaturesChocoTask();
            await chocoTask.ExecuteAsync();

            settings.ChecksumFiles = chocoTask.Features["checksumFiles"];
            settings.AutoUninstaller = chocoTask.Features["autoUninstaller"];
            settings.AllowGlobalConfirmation = chocoTask.Features["allowGlobalConfirmation"];
            settings.FailOnAutoUninstaller = chocoTask.Features["failOnAutoUninstaller"];

            return settings;
        }

        private async Task<T> LoadSettingAsync<T>(string name)
        {
            var chocoTask = new LoadSettingChocoTask(name);
            await chocoTask.ExecuteAsync();
            return (T)Convert.ChangeType(chocoTask.Setting, typeof(T));
        }

        public async Task SaveSettingsAsync(ChocoSettings settings)
        {
            await new SaveSettingsChocoTask("cacheLocation", settings.CacheLocation).ExecuteAsync();
            await new SaveSettingsChocoTask("commandExecutionTimeoutSeconds", settings.CommandExecutionTimeoutSeconds).ExecuteAsync();
            await new SaveSettingsChocoTask("containsLegacyPackageInstalls", settings.ContainsLegacyPackageInstalls).ExecuteAsync();
            await new SaveSettingsChocoTask("proxy", settings.Proxy.Address).ExecuteAsync();
            await new SaveSettingsChocoTask("proxyUser", settings.Proxy.User).ExecuteAsync();
            await new SaveSettingsChocoTask("proxyPassword", settings.Proxy.Password).ExecuteAsync();

            await new SaveFeatureChocoTask("checksumFiles", settings.ChecksumFiles).ExecuteAsync();
            await new SaveFeatureChocoTask("autoUninstaller", settings.AutoUninstaller).ExecuteAsync();
            await new SaveFeatureChocoTask("allowGlobalConfirmation", settings.AllowGlobalConfirmation).ExecuteAsync();
            await new SaveFeatureChocoTask("failOnAutoUninstaller", settings.FailOnAutoUninstaller).ExecuteAsync();
        }
    }
}
