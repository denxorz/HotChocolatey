using System;
using System.ComponentModel.DataAnnotations;

namespace HotChocolatey.Model.Save
{
    public class InstalledPackage
    {
        [Key]
        public string Id { get; set; }

        public Guid WorkStationId { get; set; }
        public virtual WorkStation WorkStation { get; set; }
    }
}
