using DrPet.Web.Data.Entities;
using DrPet.Web.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Repositories
{
    public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public DoctorRepository(DataContext context,IUserHelper userHelper)
            :base(context)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public IEnumerable<SelectListItem> GetComboDoctors()
        {
            var list = _context.Doctors.Include(d => d.User).Include(d=> d.Specialization).Where(d=> !d.IsDeleted).Select(d => new SelectListItem
            {
                Text = d.User.FullName,
                Value = d.Id.ToString()
            }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = ("Select a Doctor..."),
                Value = "0"
            });

            return list;
        }

        public async Task<Doctor> GetDoctorByUserAsync(User user)
        {
            return await _context.Doctors.Include(d => d.User).FirstOrDefaultAsync(c => c.User == user);
        }

        public IQueryable<Doctor> GetDoctors()
        {
            return _context.Doctors
                     .Include(d => d.User)
                     .Include(d => d.Specialization)
                     .Where(d => d.User.EmailConfirmed && !d.IsDeleted)
                     .OrderBy(u => u.User.FirstName);
        }

        public async Task<Doctor> GetDoctorWithUserAsync(int id)
        {
            return await _context.Doctors.Include(a => a.User).Include(d => d.Specialization).FirstOrDefaultAsync(a => a.Id == id);            
        }

        public async Task<IEnumerable<SelectListItem>> AvailableDoctors(DateTime date, int doctorId,int specId) //TESTAR COM UMA CONSULTA QUE TENHA O ID APAGADO
        {

            //NO AZURE EZXITE UM BUG COM A DATA, ADICIONEI O date.ToUniversalTime()
            var specialization = await _context.Specializations.FindAsync(specId);

            var list = _context.Appointments.Include(d => d.Doctor).ThenInclude(u => u.User).Where(a => a.StartTime == date && !a.IsDeleted);

            var doctors = _context.Doctors.Include(d => d.User).Include(d => d.Specialization).Where(d=> !d.IsDeleted && d.Specialization == specialization).Select(d => new SelectListItem
            {
                Text = $"Dr. {d.User.FullName}",
                Value = d.Id.ToString()
            }).ToList();

            foreach (var item in list)
            {
                var doc = doctors.ToList().FirstOrDefault(d => d.Value == item.Doctor.Id.ToString() && d.Value != doctorId.ToString());

                if (doc != null)
                {
                    doctors.Remove(doc);
                }
            }

            return doctors;
        }

        public async Task DeleteDoctorWithUserAsync(Doctor doctor)
        {
            var appointments = _context.Appointments.Where(a => a.Doctor == doctor && a.Status != "Completed");

            _context.Appointments.RemoveRange(appointments);

            _context.Doctors.Remove(doctor);

            _context.Users.Remove(doctor.User);

            await _context.SaveChangesAsync();
        }
    }
}
