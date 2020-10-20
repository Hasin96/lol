using App_tracker.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App_tracker.ViewModels
{
    public class CreateContainerViewModel
    {
        public int ContainerId { get; set; }
        public string RefNum { get; set; }
        public string Supplier { get; set; }
        public TimeSpan? ExpTimeOfArrival { get; set; }
        public int? ExpNumOfPallets { get; set; }
        public int? ExpNumOfUnits { get; set; }
        public TimeSpan? ActTimeOfArrival { get; set; }
        public int? ActNumOfPallets { get; set; }
        public int? ActNumOfUnits { get; set; }
        public bool CheckSheet { get; set; }
        public DateTime ArrivalDate { get; set; }
        public int Bay { get; set; }
        public string Door { get; set; }
        public int ContainerTypeId { get; set; }
        public int ContainerDepartmentId { get; set; }
        public List<ContainerComments> Comments { get; set; }
        public IEnumerable<SelectListItem> Bays { get; set; }
        public IEnumerable<ContainerTypes> ContainerTypes { get; set; }
        public IEnumerable<ContainerDepartments> ContainerDepartments { get; set; }


    }
}
