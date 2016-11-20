using HotChocolatey.Utility;
using NuGet;
using System;
using System.IO;
using System.Linq;
using PropertyChanged;

namespace HotChocolatey.Model
{
    [ImplementPropertyChanged]
    public class Package
    {
        public static readonly Package Empty = new Package(string.Empty);

        private readonly Uri noIconUri = new Uri("/HotChocolatey;component/Images/Windows10/Packaging-32.png", UriKind.Relative);

        public string Id { get; }

        private IPackage nugetPackage;

        public IPackage NugetPackage
        {
            get { return nugetPackage; }
            set
            {
                nugetPackage = value;

                if (nugetPackage != null)
                {
                    Title = string.IsNullOrWhiteSpace(NugetPackage.Title) ? NugetPackage.Id : NugetPackage.Title;
                    IsPreRelease = !NugetPackage.IsReleaseVersion();
                    Summary = NugetPackage.Summary;
                    Description = NugetPackage.Description;
                    Authors = NugetPackage.Authors.Aggregate((total, next) => total + ", " + next);
                    LicenseUrl = NugetPackage.LicenseUrl?.ToString();
                    DownloadCount = NugetPackage.DownloadCount;
                    ProjectUrl = NugetPackage.ProjectUrl?.ToString();
                    Tags = NugetPackage.Tags;
                    Dependencies = NugetPackage.DependencySets.Any()
                        ? NugetPackage.DependencySets.SelectMany(p => p.Dependencies).Distinct().Select(p => p.Id).Aggregate((working, next) => $"{working}, {next}")
                        : "<none>";

                    DetermineIconUri();

                    DescriptionAsHtml = Markdig.Markdown.ToHtml(NugetPackage.Description);
                }
            }
        }

        public SemanticVersion InstalledVersion { get; set; }
        public SemanticVersion LatestVersion { get; private set; }
        public SemanticVersion CurrentVersion => IsInstalled ? InstalledVersion : LatestVersion;
        public bool IsUpgradable { get; set; }
        public ObservableCollectionEx<SemanticVersion> Versions { get; } = new ObservableCollectionEx<SemanticVersion>();
        public ObservableCollectionEx<SemanticVersion> NewerVersions { get; } = new ObservableCollectionEx<SemanticVersion>();
        public bool IsInstalled => InstalledVersion != null;

        public string Title { get; private set; }
        public Uri Ico { get; private set; }
        public Uri SvgIco { get; private set; }
        public bool IsIcoSvg { get; private set; }
        public bool IsPreRelease { get; private set; }
        public string Summary { get; private set; }
        public string Description { get; private set; }
        public string Authors { get; private set; }
        public string LicenseUrl { get; private set; }
        public int DownloadCount { get; private set; }
        public string ProjectUrl { get; private set; }
        public string Tags { get; private set; }
        public string Dependencies { get; private set; }
        public string DescriptionAsHtml { get; private set; }

        public Package(string id)
        {
            Id = id;
            Title = id;
            Ico = noIconUri;
        }

        public void UpdateLatestVersion()
        {
            LatestVersion = Versions.FirstOrDefault();
        }

        private void DetermineIconUri()
        {
            if (NugetPackage.IconUrl == null)
            {
                Ico = noIconUri;
                return;
            }

            var stringUri = NugetPackage.IconUrl.ToString();

            if (string.IsNullOrWhiteSpace(stringUri))
            {
                Ico = noIconUri;
                return;
            }

            if (Path.GetExtension(stringUri) == ".svg")
            {
                Ico = noIconUri;
                IsIcoSvg = true;
                SvgIco = NugetPackage.IconUrl;
                return;
            }

            Ico = NugetPackage.IconUrl;
        }
    }
}
