using DrPet.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Repositories
{
    public interface IAdminRepository : IGenericRepository<Admin>
    {
        Task DeleteAdminWithUser(Admin admin);
        Task<Admin> GetAdminByUserAsync(User user);
        IQueryable<Admin> GetAdmins();
    }
}
