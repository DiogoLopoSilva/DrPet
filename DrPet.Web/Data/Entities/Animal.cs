using System;
using System.ComponentModel.DataAnnotations;

namespace DrPet.Web.Data.Entities
{
    public class Animal
    {
        public int Id { get; set; }
        
        [MaxLength(50, ErrorMessage ="The field {0} can only contain {1} characters.")]
        [Required]
        public string Name { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} can only contain {1} characters.")]
        [Required]
        public string Sex { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} can only contain {1} characters.")]
        [Required]
        public string Species { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} can only contain {1} characters.")]
        public string Breed { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} can only contain {1} characters.")]
        [Required]
        public string Color { get; set; }

        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }
    }
}
