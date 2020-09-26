using DrPet.Web.Data.Entities;
using DrPet.Web.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Repositories
{
    public class AdminRepository : GenericRepository<Admin>, IAdminRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public AdminRepository(DataContext context, IUserHelper userHelper)
            : base(context)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task<Admin> GetAdminByUserAsync(User user) //TODO VER SE TEM MAL NAO SER ASYNC
        {
            return await _context.Admins.Include(a => a.User).FirstOrDefaultAsync(c => c.User == user);
        }

        public IQueryable<Admin> GetAdmins()
        {
            return  _context.Admins
                .Include(u => u.User).Where(a => a.User.EmailConfirmed && !a.IsDeleted)
                .OrderBy(u => u.User.FirstName);
        }

        public async Task DeleteAdminWithUser(Admin admin)
        {
            _context.Admins.Remove(admin);
            _context.Users.Remove(admin.User);

            await _context.SaveChangesAsync();
        }
    }
}
