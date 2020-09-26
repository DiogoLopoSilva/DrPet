using DrPet.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Repositories
{
    public interface IClientRepository : IGenericRepository<Client>
    {
        Task DeleteClientWithUser(Client client);
        Task<Client> GetClientByUserAsync(User user);

        IQueryable<Client> GetClients();
        Task<Client> GetClientWithUserAsync(string username);
    }
}
