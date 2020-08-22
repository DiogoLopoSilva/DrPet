using DrPet.Web.Data.Entities;
using DrPet.Web.Helpers;
using Microsoft.EntityFrameworkCore;
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

        public Doctor GetDoctorByUser(User user) //TODO VER SE TEM MAL NAO SER ASYNC
        {
            return _context.Doctors.FirstOrDefault(c => c.User == user);
        }

        public async Task<IQueryable<Doctor>> GetClientsAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                return null;
            }

            if (await _userHelper.IsUserInRoleAsync(user, RoleNames.Administrator))
            {
                return _context.Doctors
                    .Include(d => d.User)
                    .OrderBy(u => u.User.FirstName);
            }

            return _context.Doctors
                .Include(d => d.User)
                .Where(c => c.User == user)
                .OrderBy(d => d.User.FirstName);
        }
    }
}
