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

        public async Task<IQueryable<Appointment>> GetAppointments(string userName)
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

            return _context.Appointments.Include(a => a.Client)
                  .ThenInclude(c => c.User)
                  .Include(a => a.Animal)
                  .Include(a => a.Doctor)
                  .ThenInclude(d => d.User)
                  .Where(a => a.Client.User == user)
                  .OrderBy(a => a.Date);
        }
    }
}
