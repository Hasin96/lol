using System;
using System.Collections.Generic;

namespace App_tracker.Models
{
    public partial class ContainerTypes
    {
        public ContainerTypes()
        {
            Containers = new HashSet<Containers>();
        }

        public byte Id { get; set; }
        public string Type { get; set; }

        public virtual ICollection<Containers> Containers { get; set; }
    }
}
