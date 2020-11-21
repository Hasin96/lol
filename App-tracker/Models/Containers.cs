using System;
using System.Collections.Generic;

namespace App_tracker.Models
{
    public partial class Containers
    {
        public Containers()
        {
            ContainerComments = new HashSet<ContainerComments>();
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
        public DateTime? ArrivalDate { get; set; }
        public byte? DepartmentId { get; set; }
        public byte TypeId { get; set; }
        public int? BayId { get; set; }
        public int? DoorId { get; set; }
        public int StatusId { get; set; }

        public virtual Bays Bay { get; set; }
        public virtual ContainerDepartments Department { get; set; }
        public virtual Doors Door { get; set; }
        public virtual ContainerStatus Status { get; set; }
        public virtual ContainerTypes Type { get; set; }
        public virtual ICollection<ContainerComments> ContainerComments { get; set; }
        public virtual ICollection<ContainerSuppliers> ContainerSuppliers { get; set; }
    }
}
