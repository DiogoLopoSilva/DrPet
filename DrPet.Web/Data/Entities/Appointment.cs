using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Entities
{
    public class Appointment : IEntity
    {
        public int Id { get; set; }

        public Client Client { get; set; }

        public Animal Animal { get; set; }

        public Doctor Doctor { get; set; }

        public DateTime Date { get; set; }

        public string Notes { get; set; }
    }
}
