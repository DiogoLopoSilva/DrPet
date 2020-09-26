using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Entities
{
    public class User : IdentityUser
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }
     
        public string Gender { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Document No.")]
        public string DocumentNumber { get; set; }

        public string StreeName { get; set; }

        public string Location { get; set; }

        public string PostalCode { get; set; }

        [Display(Name = "Name")]
        public string FullName { get { return $"{FirstName} {LastName}"; } }

        [Display(Name = "Date Created")]
        [DataType(DataType.Date)]
        public DateTime DateCreated { get; set; } = DateTime.Today; //TODO VER SE ISTO NAO CAUSA CONFLITOS COM UPDATES DE USERS

        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        public bool IsDeleted { get; set; }

    }
}
