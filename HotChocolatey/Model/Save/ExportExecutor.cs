using System;
using System.Collections.Generic;
using System.Linq;
using NuGet;

namespace HotChocolatey.Model.Save
{
    class ExportExecutor
    {
        public void Export(string filename, List<Package> packages)
        {
            using (ListContext db = new ListContext(filename))
            {
                var workstation = db.WorkStations.FirstOrDefault(w => w.Name == Environment.MachineName);
                if (workstation == null)
                {
                    workstation = new WorkStation { Id = Guid.NewGuid(), Name = Environment.MachineName, InstalledPackages = new List<InstalledPackage>() };
                    db.WorkStations.Add(workstation);
                }

                workstation.InstalledPackages.Clear();
                workstation.InstalledPackages.AddRange(packages.Select(p => new InstalledPackage { Id = p.Id }));
                db.SaveChanges();
            }
        }

        public List<WorkStation> Import(string filename)
        {
            using (ListContext db = new ListContext(filename))
            {
                return db.WorkStations.Include("InstalledPackages").ToList();
            }
        }
    }
}
