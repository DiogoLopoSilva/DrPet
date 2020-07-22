using System;
using System.ComponentModel.DataAnnotations;

namespace DrPet.Web.Data.Entities
{
    public class Animal
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Sex { get; set; }

        public string Species { get; set; }

        public string Breed { get; set; }

        public string Color { get; set; }

        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }
    }
}
