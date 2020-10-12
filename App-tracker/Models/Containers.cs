using System;
using System.Collections.Generic;

namespace App_tracker.Models
{
    public partial class Containers
    {
        public Containers()
        {
            ContainerSuppliers = new HashSet<ContainerSuppliers>();
        }

        public int Id { get; set; }
        public string RefNum { get; set; }
        public TimeSpan? ExpTimeOfArrival { get; set; }
        public int? ExpNumOfPallets { get; set; }
        public int? ExpNumOfUnits { get; set; }
        public TimeSpan? ActTimeOfArrival { get; set; }
        public int? ActNumOfPallets { get; set; }
        public int? ActNumOfUnits { get; set; }
        public int? ItemDiscrepancy { get; set; }
        public bool CheckSheet { get; set; }
        public DateTime ArrivalDate { get; set; }
        public int Bay { get; set; }
        public byte? DepartmentId { get; set; }
        public byte TypeId { get; set; }

        public virtual ContainerDepartments Department { get; set; }
        public virtual ContainerTypes Type { get; set; }
        public virtual ICollection<ContainerSuppliers> ContainerSuppliers { get; set; }
    }
}
