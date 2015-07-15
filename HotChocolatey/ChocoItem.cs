
using System;
using System.ComponentModel;
using NuGet;
using System.IO;
using System.Collections.Generic;

namespace HotChocolatey
{
    [Magic]
    public class ChocoItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public IPackage Package { get; }

        public string Name => Package.Id;
        public SemanticVersion InstalledVersion { get; private set; }
        public SemanticVersion LatestVersion { get; private set; }
        public bool IsUpgradable { get; private set; }
        public List<SemanticVersion> Versions { get; set; }

        public List<IAction> Actions { get; set; }

        public string Title => Package.Title;
        public Uri Ico => Package.IconUrl == null || Path.GetExtension(Package.IconUrl.ToString()) == ".svg" ? noIconUri : Package.IconUrl;
        public bool IsPreRelease => !Package.IsReleaseVersion();
        public string Summary => Package.Summary;
        public string Description => Package.Description;

        public bool IsInstalled => InstalledVersion != null;

        private readonly Uri noIconUri = new Uri("/HotChocolatey;component/Images/chocolateyicon.gif", UriKind.Relative);

        public ChocoItem(IPackage package)
        {
            Package = package;
        }

        public ChocoItem(IPackage package, SemanticVersion installedVersion, SemanticVersion latestVersion)
            : this(package)
        {
            InstalledVersion = installedVersion;
            LatestVersion = latestVersion;
            IsUpgradable = installedVersion != latestVersion;
        }

        private void RaisePropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

