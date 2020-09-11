using DrPet.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Entities
{
    public class EditParams
    {
        public string key { get; set; }
        public string action { get; set; }
        public List<AppointmentViewModel> added { get; set; }
        public List<AppointmentViewModel> changed { get; set; }
        public List<AppointmentViewModel> deleted { get; set; }
        public AppointmentViewModel value { get; set; }
    }
}
