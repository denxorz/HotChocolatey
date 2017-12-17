using System;
using NuGet;

namespace HotChocolatey.Model.ChocoTask
{
    public class UpdateLocalChocoTask : BaseChocoTask
    {
        private readonly PackageRepo repo;
        private readonly Action<Package> addPackageCallback;

        public UpdateLocalChocoTask(bool includePreReleases, PackageRepo repo, Action<Package> addPackageCallback)
        {
            this.repo = repo;
            this.addPackageCallback = addPackageCallback;

            Config.CommandName = "list";
            Config.Prerelease = includePreReleases;
            Config.ListCommand.LocalOnly = true;
        }

        protected override Action<string> GetOutputLineCallback() => UpdateLocalPackage;

        private void UpdateLocalPackage(string chocoOutput)
        {
            var tmp = chocoOutput.Split('|', ' ');
            if (tmp.Length != 2) return;

            var p = repo.GetPackage(tmp[0]);
            p.InstalledVersion = new SemanticVersion(tmp[1]);
            addPackageCallback(p);
        }
    }
}