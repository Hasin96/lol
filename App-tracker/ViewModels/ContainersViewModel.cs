using App_tracker.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace App_tracker.ViewModels
{
    public class ContainersViewModel
    {
        public ContainersViewModel()
        {
            ContainerStatuses = new List<ContainerStatus>();
            Containers = new List<Containers>();
            Bays = new List<SelectListItem>();
        }

        public List<ContainerStatus> ContainerStatuses { get; set; }
        public List<Containers> Containers { get; set; }
        public List<SelectListItem> Bays { get; set; }
        public List<SelectListItem> Doors { get; set; }
        public bool PromptForStartDate { get; set; }
        public DateTime StartOf4DayWeekDate { get; set; }

        public string ToString(Containers container)
        {
            string suppliers = "";
            if (container.ContainerSuppliers.Count() > 0)
            {
                for(int i = 0; i < container.ContainerSuppliers.Count(); i++)
                {
                    if (i == 0)
                        suppliers += container.ContainerSuppliers.ElementAt(i).Supplier.Supplier;
                    else if (i > 0)
                        suppliers += "/" + container.ContainerSuppliers.ElementAt(i).Supplier.Supplier;
                    
                }
            }

            return suppliers;
        }
    }
}