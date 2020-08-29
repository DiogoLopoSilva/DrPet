using DrPet.Web.Data.Entities;
using DrPet.Web.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Repositories
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public AppointmentRepository(DataContext context, IUserHelper userHelper)
            : base(context)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task<IQueryable<Appointment>> GetAppointmentsAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);

            if (user == null)
            {
                return null;
            }

            if (await _userHelper.IsUserInRoleAsync(user, RoleNames.Administrator))
            {
                return _context.Appointments.Include(a => a.Client)
                      .ThenInclude(c => c.User)
                      .Include(a => a.Animal)
                      .Include(a => a.Doctor)
                      .ThenInclude(d => d.User)
                      .OrderBy(a => a.Date);
            }

            if (await _userHelper.IsUserInRoleAsync(user, RoleNames.Doctor))
            {
                return _context.Appointments.Include(a => a.Client)
                 .ThenInclude(c => c.User)
                 .Include(a => a.Animal)
                 .Include(a => a.Doctor)
                 .ThenInclude(d => d.User)
                 .Where(a => a.Doctor.User == user)
                 .OrderBy(a => a.Date);
            }

            return _context.Appointments.Include(a => a.Client)
                  .ThenInclude(c => c.User)
                  .Include(a => a.Animal)
                  .Include(a => a.Doctor)
                  .ThenInclude(d => d.User)
                  .Where(a => a.Client.User == user)
                  .OrderBy(a => a.Date);
        }

        public async Task<IQueryable<AppointmentTemp>> GetAppointmentsTempAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);

            if (user == null)
            {
                return null;
            }

            return _context.AppointmentsTemp.Include(a => a.Client)
                     .ThenInclude(c => c.User)
                     .Include(a => a.Animal)
                     .Include(a => a.Doctor)
                     .ThenInclude(d => d.User)
                     .OrderBy(a => a.Date);
        }

        public async Task CreateAppointmentTemp(AppointmentTemp appointmentTemp)
        {
            await _context.AppointmentsTemp.AddAsync(appointmentTemp);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAppointmentTempAsync(int id)
        {
            var appointmentTemp = await _context.AppointmentsTemp.FindAsync(id);
            if (appointmentTemp == null)
            {
                return;
            }

            _context.AppointmentsTemp.Remove(appointmentTemp);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ConfirmAppointmentAsync(int id)
        {
            var appointmentTemp = await _context.AppointmentsTemp.Where(a => a.Id == id).Include(a => a.Client)
                     .ThenInclude(c => c.User)
                     .Include(a => a.Animal)
                     .Include(a => a.Doctor)
                     .ThenInclude(d => d.User)
                     .FirstOrDefaultAsync(a => a.Id == id);

            if (appointmentTemp==null)
            {
                return false;
            }

            var appointment = new Appointment
            {
                Client = appointmentTemp.Client,
                Animal=appointmentTemp.Animal,
                Doctor=appointmentTemp.Doctor,
                Date=appointmentTemp.Date,
                Notes=appointmentTemp.Notes
            };

            await _context.Appointments.AddAsync(appointment);
            _context.AppointmentsTemp.Remove(appointmentTemp);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
