using System;
using System.Collections.Generic;

namespace App_tracker.Models
{
    public partial class Bays
    {
        public Bays()
        {
            Containers = new HashSet<Containers>();
        }

        public int Id { get; set; }
        public short Bay { get; set; }

        public virtual ICollection<Containers> Containers { get; set; }
    }
}
