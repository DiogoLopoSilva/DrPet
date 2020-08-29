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
        //[Display(Name = "Product")]
        //[Range(1, int.MaxValue, ErrorMessage = "You must select a Client.")]
        //public int ClientId { get; set; }

        //public IEnumerable<SelectListItem> Clients { get; set; }

        [Display(Name = "Product")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select an Animal.")]
        public int AnimalId { get; set; }

        public IEnumerable<SelectListItem> Animals { get; set; }

        [Display(Name = "Product")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Doctor.")]
        public int DoctorId { get; set; }

        public IEnumerable<SelectListItem> Doctors { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        public string Notes { get; set; }

        public User User { get; set; }
    }
}
