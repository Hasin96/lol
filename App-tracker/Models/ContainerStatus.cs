using System;
using System.Collections.Generic;

namespace App_tracker.Models
{
    public partial class ContainerStatus
    {
        public ContainerStatus()
        {
            Containers = new HashSet<Containers>();
        }

        public int Id { get; set; }
        public string Status { get; set; }

        public virtual ICollection<Containers> Containers { get; set; }
    }
}
