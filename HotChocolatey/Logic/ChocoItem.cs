using HotChocolatey.ViewModel;
using NuGet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace HotChocolatey.Logic
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

        public string Title => string.IsNullOrWhiteSpace(Package.Title) ? Package.Id : Package.Title;
        public Uri Ico => Package.IconUrl == null || Path.GetExtension(Package.IconUrl.ToString()) == ".svg" ? noIconUri : Package.IconUrl;
        public bool IsPreRelease => !Package.IsReleaseVersion();
        public string Summary => Package.Summary;
        public string Description => Package.Description;

        public string Authors => Package.Authors.Aggregate((total, next) => total + ", " + next);
        public string LicenseUrl => Package.LicenseUrl?.ToString();
        public int DownloadCount => Package.DownloadCount;
        public string ProjectUrl => Package.ProjectUrl?.ToString();
        public string Tags => Package.Tags;
        public string Dependencies => string.Empty; // TODO

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

