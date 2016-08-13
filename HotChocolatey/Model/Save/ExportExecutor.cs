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
                var workstation = db.WorkStations.Include(nameof(WorkStation.InstalledPackages)).FirstOrDefault(w => w.Name == Environment.MachineName);
                if (workstation == null)
                {
                    workstation = new WorkStation { UniqueId = Guid.NewGuid(), Name = Environment.MachineName, InstalledPackages = new List<InstalledPackage>() };
                    db.WorkStations.Add(workstation);
                    db.SaveChanges();
                }

                workstation.InstalledPackages.Clear();
                workstation.InstalledPackages.AddRange(packages.Select(p => new InstalledPackage { Id = p.Id, WorkStation = workstation }));
                db.SaveChanges();
            }
        }

        public List<WorkStation> Import(string filename)
        {
            using (ListContext db = new ListContext(filename))
            {
                return db.WorkStations.Include(nameof(WorkStation.InstalledPackages)).ToList();
            }
        }
    }
}
