using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace HotChocolatey.Model.Save
{
    class ExportExecutor
    {
        public void Export(string filename, IEnumerable<Package> packages)
        {
            using (ListContext db = new ListContext(filename))
            {
                db.Database.Migrate();

                var workstation = db.WorkStations.FirstOrDefault(w => w.Name == Environment.MachineName);
                if (workstation == null)
                {
                    workstation = new WorkStation { UniqueId = Guid.NewGuid(), Name = Environment.MachineName, InstalledPackages = new List<InstalledPackage>() };
                    db.WorkStations.Add(workstation);
                    db.SaveChanges();
                }

                db.Entry(workstation)
                    .Collection(b => b.InstalledPackages)
                    .Load();

                workstation.InstalledPackages.Clear();
                db.SaveChanges();

                workstation.InstalledPackages.AddRange(packages.Select(p => new InstalledPackage { Id = p.Id, WorkStation = workstation }));
                db.SaveChanges();
            }
        }

        public List<WorkStation> Import(string filename)
        {
            using (ListContext db = new ListContext(filename))
            {
                db.Database.Migrate();

                return db.WorkStations
                            .Include(p => p.InstalledPackages)
                            .ToList();
            }
        }
    }
}
