using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotChocolatey.Model.Save
{
    public class WorkStation
    {
        [Key]
        public int Id { get; set; }

        public Guid UniqueId { get; set; }

        public string Name { get; set; }

        public List<InstalledPackage> InstalledPackages { get; set; }
    }
}
