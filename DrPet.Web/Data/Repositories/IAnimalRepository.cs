using DrPet.Web.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Repositories
{
    public interface IAnimalRepository :  IGenericRepository<Animal> //TODO O ANIMALREPOSITORY IMPLEMENTE O GENERIC PARA GARANTIR QUE DENTRO DO ANIMAL REPOSITORY OS METODOS DE GENERO SAO IMPLEMENTADOS(ACHO)
    {
        Task<IQueryable<Animal>> GetAnimalsAsync(string userName);

        IEnumerable<SelectListItem> GetComboAnimals(string userName);

        Animal GetAnimalWithUser(int id);
    }
}
