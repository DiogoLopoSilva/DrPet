using DrPet.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Models
{
    public class AppointmentsWithTempView
    {
        public IEnumerable<Appointment> Appointments { get; set; }

        public IEnumerable<AppointmentTemp> AppointmentsTemp { get; set; }
    }
}
