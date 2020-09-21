using DrPet.Web.Data.Entities;
using Microsoft.AspNetCore.Http;
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
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Required]
        public string DocumentNumber { get; set; }

        [Required]
        public string StreetName { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string PostalCode { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Display(Name = "Date Created")]
        [DataType(DataType.Date)]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        public IEnumerable<Appointment> Appointments { get; set; }

        public string Role { get; set; }

        public Doctor Doctor { get; set; }

        [Display(Name = "Name")]
        public string FullName { get { return $"{FirstName} {LastName}"; } }
    }
}
