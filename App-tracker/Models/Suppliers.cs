using System;
using System.Collections.Generic;

namespace App_tracker.Models
{
    public partial class Suppliers
    {
        public Suppliers()
        {
            ContainerSuppliers = new HashSet<ContainerSuppliers>();
        }

        public int Id { get; set; }
        public string Supplier { get; set; }

        public virtual ICollection<ContainerSuppliers> ContainerSuppliers { get; set; }
    }
}
