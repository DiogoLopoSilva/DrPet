using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Entities
{
    public class Appointment : IEntity
    {
        public int Id { get; set; }

        [Required]
        public Client Client { get; set; }

        [Required]
        public Animal Animal { get; set; }

        [Required]
        public Doctor Doctor { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; } //TODO MUDAR PARA LOCAL TIME

        public string Notes { get; set; }
    }
}
