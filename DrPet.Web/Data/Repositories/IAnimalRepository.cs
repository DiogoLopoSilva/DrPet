using DrPet.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Repositories
{
    public interface IAnimalRepository :  IGenericRepository<Animal>
    {
        Task<IQueryable<Animal>> GetAnimalsAsync(string userName);
    }
}
