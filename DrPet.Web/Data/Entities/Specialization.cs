using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Entities
{
    public class Specialization : IEntity
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Specialization")]
        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters.")]
        public string Name { get; set; }

        public bool IsDeleted { get; set; }
    }
}
