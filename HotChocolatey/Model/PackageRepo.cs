using System.Collections.Generic;
using System.Linq;

namespace HotChocolatey.Model
{
    public class PackageRepo
    {
        private readonly object installedPackagesLock = new object();
        private readonly Dictionary<string, Package> installedPackages = new Dictionary<string, Package>();

        public Package GetPackage(string id)
        {
            lock (installedPackagesLock)
            {
                if (!installedPackages.ContainsKey(id))
                {
                    installedPackages[id] = new Package(id);
                }
                return installedPackages[id];
            }
        }

        public void ClearLocalVersions()
        {
            lock (installedPackagesLock)
            {
                installedPackages.Values.ToList().ForEach(p =>
                {
                    p.InstalledVersion = null;
                    p.IsUpgradable = false;
                });
            }
        }
    }
}
