using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotChocolatey.Model.Save
{
    public class WorkStation
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<InstalledPackage> InstalledPackages { get; set; }
    }
}
