using HotChocolatey.Utility;
using HotChocolatey.ViewModel;
using NuGet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HotChocolatey.Model
{
    [Magic]
    public class Package
    {
        private readonly Uri noIconUri = new Uri("/HotChocolatey2;component/Images/chocolateyicon.gif", UriKind.Relative);

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
                    Ico = NugetPackage.IconUrl == null || Path.GetExtension(NugetPackage.IconUrl.ToString()) == ".svg" ? noIconUri : NugetPackage.IconUrl;
                    IsPreRelease = !NugetPackage.IsReleaseVersion();
                    Summary = NugetPackage.Summary;
                    Description = NugetPackage.Description;
                    Authors = NugetPackage.Authors.Aggregate((total, next) => total + ", " + next);
                    LicenseUrl = NugetPackage.LicenseUrl?.ToString();
                    DownloadCount = NugetPackage.DownloadCount;
                    ProjectUrl = NugetPackage.ProjectUrl?.ToString();
                    Tags = NugetPackage.Tags;
                    Dependencies = string.Empty; // TODO : issue #27

                    DescriptionAsHtml = new MarkdownSharp.Markdown().Transform(NugetPackage.Description);
                }
            }
        }
        public SemanticVersion InstalledVersion { get; set; }
        public SemanticVersion LatestVersion { get; private set; }
        public bool IsUpgradable { get; set; }
        public List<SemanticVersion> Versions { get; } = new List<SemanticVersion>();
        public ObservableCollectionEx<IAction> Actions { get; set; } = new ObservableCollectionEx<IAction>();
        public IAction DefaultAction => Actions.Any() ? Actions.First() : null;
        public bool IsInstalled => InstalledVersion != null;

        public string Title { get; private set; }
        public Uri Ico { get; private set; }
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

        public void GenerateActions()
        {
            var actions = new List<IAction>();

            if (IsInstalled)
            {
                if (IsUpgradable)
                {
                    actions.Add(new UpgradeAction(this));
                }

                actions.Add(new UninstallAction(this));
            }
            else
            {
                actions.Add(new InstallAction(this));
            }

            Actions.ClearAndAddRange(actions);
        }

        private class InstallAction : IAction
        {
            private readonly Package package;

            public InstallAction(Package package)
            {
                this.package = package;
                Versions = package.Versions.OrderByDescending(t => t).ToList();
            }

            public string Name { get; } = "Install";
            public List<SemanticVersion> Versions { get; }

            public async Task Execute(ChocoExecutor chocoExecutor, SemanticVersion specificVersion)
            {
                await chocoExecutor.Install(package, specificVersion);
            }

            public override string ToString()
            {
                return Name;
            }
        }

        private class UninstallAction : IAction
        {
            private readonly Package package;

            public UninstallAction(Package package)
            {
                this.package = package;
                Versions = new List<SemanticVersion> { package.InstalledVersion };
            }

            public string Name { get; } = "Uninstall";
            public List<SemanticVersion> Versions { get; }

            public async Task Execute(ChocoExecutor chocoExecutor, SemanticVersion specificVersion)
            {
                await chocoExecutor.Uninstall(package);
            }

            public override string ToString()
            {
                return Name;
            }
        }

        private class UpgradeAction : IAction
        {
            private readonly Package package;

            public UpgradeAction(Package package)
            {
                this.package = package;
                Versions = package.Versions.Where(t => t > package.InstalledVersion).OrderByDescending(t => t).ToList();
            }

            public string Name { get; } = "Upgrade";
            public List<SemanticVersion> Versions { get; }

            public async Task Execute(ChocoExecutor chocoExecutor, SemanticVersion specificVersion)
            {
                await chocoExecutor.Upgrade(package, specificVersion);
            }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}

