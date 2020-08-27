using DrPet.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Repositories
{
    public interface IAdminRepository : IGenericRepository<Admin>
    {
        Admin GetAdminByUser(User user);
        IQueryable<Admin> GetAdmins();
    }
}
