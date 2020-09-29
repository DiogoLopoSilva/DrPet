using System;
using System.ComponentModel.DataAnnotations;

namespace DrPet.Web.Data.Entities
{
    public class Animal :IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage ="The field {0} can only contain {1} characters.")]        
        public string Name { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} can only contain {1} characters.")]      
        public string Sex { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} can only contain {1} characters.")]       
        public string Species { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} can only contain {1} characters.")]
        public string Breed { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name="Image")]
        public string ImageUrl { get; set; }

        [Display(Name="Owner")]
        public User User { get; set; }

        public bool IsDeleted { get; set; }
    }
}
