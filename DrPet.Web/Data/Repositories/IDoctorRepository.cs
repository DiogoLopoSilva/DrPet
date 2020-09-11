using DrPet.Web.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        IEnumerable<SelectListItem> GetComboDoctors();

        Doctor GetDoctorWithUser(int id);

        IEnumerable<SelectListItem> AvailableDoctors(DateTime date, int doctorId);
    }
}
