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
        IEnumerable<SelectListItem> GetComboSpecializations();
    }
}
