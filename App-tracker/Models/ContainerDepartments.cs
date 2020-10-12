using System;
using System.Collections.Generic;

namespace App_tracker.Models
{
    public partial class ContainerDepartments
    {
        public ContainerDepartments()
        {
            Containers = new HashSet<Containers>();
        }

        public byte Id { get; set; }
        public string Department { get; set; }

        public virtual ICollection<Containers> Containers { get; set; }
    }
}
