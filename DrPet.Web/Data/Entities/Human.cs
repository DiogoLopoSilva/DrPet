using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Entities
{
    public abstract class Human : IEntity
    {
        public int Id { get; set; }

        public User User { get; set; }
    }
}
