using DrPet.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Repositories
{
    public interface IDoctorRepository : IGenericRepository<Doctor>
    {
        Doctor GetDoctorByUser(User user);

        Task<IQueryable<Doctor>> GetDoctorsAsync(string userName);
    }
}
