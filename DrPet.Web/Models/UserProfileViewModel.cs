using DrPet.Web.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Models
{
    public class UserProfileViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters.")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters.")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Required]
        [Display(Name = "NIF")]
        [StringLength(9, MinimumLength =9, ErrorMessage ="Invalid NIF")]
        public string DocumentNumber { get; set; }

        [Required]
        [Display(Name = "Street Name")]
        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters.")]
        public string StreetName { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters.")]
        public string Location { get; set; }

        [Required]
        [Display(Name = "Postal Code")]
        [RegularExpression("^\\d{4}-\\d{3}$",ErrorMessage ="Wrong Format")]
        public string PostalCode { get; set; }

        [Required]
        [Display(Name = "Phone")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "Invalid Phone")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Date Created")]
        [DataType(DataType.Date)]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        public IEnumerable<Appointment> Appointments { get; set; }

        public IEnumerable<Animal> Animals { get; set; }

        public Doctor Doctor { get; set; }

        [Display(Name = "Specialization")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Specialization")]
        public int SpecializationId { get; set; }

        public IEnumerable<SelectListItem> Specializations { get; set; }

        [Display(Name = "Name")]
        public string FullName { get { return $"{FirstName} {LastName}"; } }
    }
}
