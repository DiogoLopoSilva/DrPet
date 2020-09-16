using DrPet.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Models
{
    public class AppointmentIndexViewModel
    {
        public IEnumerable<Appointment> CompleteList { get; set; }

        public IEnumerable<Appointment> ConfirmedList { get; }

        public IEnumerable<Appointment> WaitingAprovalList { get; }

        public AppointmentIndexViewModel(IEnumerable<Appointment> list)
        {
            CompleteList = list;

            ConfirmedList = CompleteList.Where(a => a.Status == "Confirmed");

            WaitingAprovalList = CompleteList.Where(a => a.Status == "Waiting");
        }

    }
}
