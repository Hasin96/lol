using System;
using System.Collections.Generic;

namespace App_tracker.Models
{
    public partial class ContainerSuppliers
    {
        public int ContainerId { get; set; }
        public int SupplierId { get; set; }

        public virtual Containers Container { get; set; }
        public virtual Suppliers Supplier { get; set; }
    }
}
