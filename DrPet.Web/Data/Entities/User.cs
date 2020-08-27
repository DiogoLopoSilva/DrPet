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
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        public string DocumentNumber { get; set; }

        public string StreeName { get; set; }

        public string Location { get; set; }

        public string PostalCode { get; set; }

        public string Phone { get; set; }

        public string FullName { get { return $"{FirstName} {LastName}"; } }
    }
}
