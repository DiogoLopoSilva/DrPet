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

        public Admin GetAdminByUser(User user) //TODO VER SE TEM MAL NAO SER ASYNC
        {
            return _context.Admins.FirstOrDefault(c => c.User == user);
        }

        public IQueryable<Admin> GetAdmins()
        {
            return  _context.Admins
                .Include(d => d.User)
                .OrderBy(u => u.User.FirstName);
        }
    }
}
