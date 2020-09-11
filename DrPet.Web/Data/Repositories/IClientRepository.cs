using DrPet.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Repositories
{
    public interface IClientRepository : IGenericRepository<Client>
    {
        Client GetClientByUser(User user);

        Task<IQueryable<Client>> GetClientsAsync(string userName);
        Client GetClientWithUser(string username);
    }
}
