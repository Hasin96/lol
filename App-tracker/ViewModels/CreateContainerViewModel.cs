using App_tracker.Controllers;
using App_tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace App_tracker.ViewModels
{
    public class CreateContainerViewModel
    {

        public CreateContainerViewModel()
        {
            Comments = new List<ContainerComments>();
        }

        public int ContainerId { get; set; }
        [Required]
        [Display(Name = "Reference / Appointment number")]
        public string RefNum { get; set; }
        public TimeSpan? ExpTimeOfArrival { get; set; }
        public int? ExpNumOfPallets { get; set; }
        public int? ExpNumOfUnits { get; set; }
        public TimeSpan? ActTimeOfArrival { get; set; }
        public int? ActNumOfPallets { get; set; }
        public int? ActNumOfUnits { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public int? BayId { get; set; }
        public int? DoorId { get; set; }
        public byte? ContainerTypeId { get; set; }
        public byte? ContainerDepartmentId { get; set; }
        public List<int?> SupplierIds { get; set; }
        public List<ContainerComments> Comments { get; set; }
        public IEnumerable<SelectListItem> Doors { get; set; }
        public IEnumerable<SelectListItem> Suppliers { get; set; }
        public IEnumerable<SelectListItem> Bays { get; set; }
        public IEnumerable<ContainerTypes> ContainerTypes { get; set; }
        public IEnumerable<ContainerDepartments> ContainerDepartments { get; set; }
        public string Action {
            get
            {
                return (ContainerId != 0) ? "Update" : "Create";
            }
        }

    }
}
