using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotChocolatey.Model.Save
{
    public class InstalledPackage
    {
        [Key, Column(Order = 0)]
        public string Id { get; set; }

        [Key, Column(Order = 1)]
        public int WorkStationId { get; set; }
        public virtual WorkStation WorkStation { get; set; }
    }
}
