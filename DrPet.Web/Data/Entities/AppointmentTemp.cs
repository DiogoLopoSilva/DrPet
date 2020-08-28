using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Entities
{
    public class AppointmentTemp: IEntity
    {
        public int Id { get; set; }

        [Required]
        public Client Client { get; set; }

        [Required]
        public Animal Animal { get; set; }

        [Required]
        public Doctor Doctor { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public string Notes { get; set; }
    }
}
