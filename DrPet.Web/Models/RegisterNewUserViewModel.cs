using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Models
{
    public class RegisterNewUserViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }


        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; } 

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string Confirm { get; set; }

        [Display(Name = "Role")] //NAO TENHO O RANGE PORQUE POR DEFAULT ESTA SEMPRE SELECIONADO UM VALOR
        public int RoleId { get; set; }

        public IEnumerable<SelectListItem> Roles { get; set; }

        [Display(Name = "Specialization")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Specialization")]
        public int SpecializationId { get; set; }

        public IEnumerable<SelectListItem> Specializations { get; set; }
    }
}
