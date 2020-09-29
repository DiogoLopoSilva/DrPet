using DrPet.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Models
{
    public class AnimalDetailsViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters.")]
        public string Name { get; set; }

        [Display(Name = "Sex")]
        public string Sex { get; set; }

        [Required]
        [Display(Name = "Species")]
        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters.")]
        public string Species { get; set; }

        [Required]
        [Display(Name = "Breed")]
        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters.")]
        public string Breed { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Owner")]
        public User User { get; set; }

        public IEnumerable<Appointment> Appointments { get; set; }

        [Display(Name = "Image")]
        public string ImageUrl { get; set; }
    }
}
