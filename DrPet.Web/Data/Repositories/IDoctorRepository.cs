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
        Task<Doctor> GetDoctorByUserAsync(User user);

        IQueryable<Doctor> GetDoctors();

        IEnumerable<SelectListItem> GetComboDoctors();

        Task<Doctor> GetDoctorWithUserAsync(int id);

        IEnumerable<SelectListItem> AvailableDoctors(DateTime date, int doctorId);
        Task DeleteDoctorWithUserAsync(Doctor doctor);
    }
}
