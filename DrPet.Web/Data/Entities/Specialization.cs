using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Entities
{
    public class Specialization : IEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
