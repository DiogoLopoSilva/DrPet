using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Entities
{
    public class Doctor : Human
    {
        public string Specialization { get; set; }

    }
}
