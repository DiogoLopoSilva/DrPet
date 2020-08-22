using DrPet.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Repositories
{
    public class AdminRepository : GenericRepository<Admin>, IAdminRepository
    {
        private readonly DataContext _context;

        public AdminRepository(DataContext context)
            :base(context)
        {
            _context = context;
        }

        public Admin GetAdminByUser(User user) //TODO VER SE TEM MAL NAO SER ASYNC
        {
            return _context.Admins.FirstOrDefault(c => c.User == user);
        }
    }
}
