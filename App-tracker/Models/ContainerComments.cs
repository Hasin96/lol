using System;
using System.Collections.Generic;

namespace App_tracker.Models
{
    public partial class ContainerComments
    {
        public int Id { get; set; }
        public int ContainerId { get; set; }
        public string Comment { get; set; }

        public virtual Containers Container { get; set; }
    }
}
