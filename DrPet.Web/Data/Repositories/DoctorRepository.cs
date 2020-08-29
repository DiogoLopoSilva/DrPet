using DrPet.Web.Data.Entities;
using DrPet.Web.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public IEnumerable<SelectListItem> GetComboDoctors()
        {
            var list = _context.Doctors.Include(d => d.User).Select(d => new SelectListItem
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

        public Doctor GetDoctorByUser(User user) //TODO VER SE TEM MAL NAO SER ASYNC
        {
            return _context.Doctors.FirstOrDefault(c => c.User == user);
        }

        public async Task<IQueryable<Doctor>> GetDoctorsAsync(string userName)
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

        public Doctor GetDoctorWithUser(int id)
        {
            return _context.Doctors.Include(a => a.User).FirstOrDefault(a => a.Id == id);            
        }
    }
}
