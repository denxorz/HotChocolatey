using System;
using NuGet;

namespace HotChocolatey.Model.ChocoTask
{
    internal class UpdateLocalChocoTask : BaseChocoTask
    {
        private readonly bool includePreReleases;
        private readonly PackageRepo repo;
        private readonly Action<Package> addPackageCallback;

        public UpdateLocalChocoTask(bool includePreReleases, PackageRepo repo, Action<Package> addPackageCallback) 
        {
            this.includePreReleases = includePreReleases;
            this.repo = repo;
            this.addPackageCallback = addPackageCallback;
        }

        protected override string GetCommand()=>"list";

        protected override string GetParameters()
        {
            var includePreRelease = includePreReleases ? "--prerelease" : string.Empty;
            return $"--localonly {includePreRelease}";
        }

        protected override Action<string> GetOutputLineCallback() => UpdateLocalPackage;
        
        private void UpdateLocalPackage(string chocoOutput)
        {
            var tmp = chocoOutput.Split('|', ' ');
            var p = repo.GetPackage(tmp[0]);
            p.InstalledVersion = new SemanticVersion(tmp[1]);
            addPackageCallback(p);
        }
    }
}