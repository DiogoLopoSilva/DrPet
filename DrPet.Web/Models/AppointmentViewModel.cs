using DrPet.Web.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Models
{
    public class AppointmentViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Client")]
        public string ClientUsername{ get; set; }

        [Display(Name = "Animal")]
        public int AnimalId { get; set; }

        [Display(Name = "Doctor")]
        public int DoctorId { get; set; }

        [Display(Name = "Specialization")]
        public int SpecializationId { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime EndTime { get { return this.StartTime.AddMinutes(30); } }

        public string ClientDescription { get; set; }

        public string Notes { get; set; }
    }
}
