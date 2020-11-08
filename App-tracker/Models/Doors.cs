using System;
using System.Collections.Generic;

namespace App_tracker.Models
{
    public partial class Doors
    {
        public Doors()
        {
            Containers = new HashSet<Containers>();
        }

        public int Id { get; set; }
        public string Door { get; set; }

        public virtual ICollection<Containers> Containers { get; set; }
    }
}
