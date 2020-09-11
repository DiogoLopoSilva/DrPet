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
        [Display(Name = "First Name")]
        public string FirstName { get; set; }


        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }


        [Required]
        public string Password { get; set; }


        [Required]
        [Compare("Password")]
        public string Confirm { get; set; }

        [Display(Name = "Role")] //NAO TENHO O RANGE PORQUE POR DEFAULT ESTA SEMPRE SELECIONADO UM VALOR
        public int RoleId { get; set; }

        public IEnumerable<SelectListItem> Roles { get; set; }
    }
    public enum RolesEnum //TODO ORDERNAR POR CLIENT,DOCTOR,ADMIN
    {
        Admin = 1,
        Client = 2,
        Doctor = 3
    }
}
