using DrPet.Web.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Repositories
{
    public interface ISpecializationRepository : IGenericRepository<Specialization>
    {
        Task CreateUniqueAsync(Specialization addSpecialization);
        Task DeleteByIdAsync(int id);
        Specialization GetById(int Id);
        IEnumerable<SelectListItem> GetComboSpecializations();
        IQueryable GetSpecializations();
    }
}
