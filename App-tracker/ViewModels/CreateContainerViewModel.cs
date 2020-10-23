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
        public Containers Container { get; set; }
        public int ContainerTypeId { get; set; }
        public int ContainerDepartmentId { get; set; }
        public List<ContainerComments> Comments { get; set; }
        public IEnumerable<SelectListItem> Bays { get; set; }
        public IEnumerable<ContainerTypes> ContainerTypes { get; set; }
        public IEnumerable<ContainerDepartments> ContainerDepartments { get; set; }


    }
}
