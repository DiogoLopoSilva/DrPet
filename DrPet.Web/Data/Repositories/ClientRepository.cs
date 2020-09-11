using DrPet.Web.Data.Entities;
using DrPet.Web.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Repositories
{
    public class ClientRepository : GenericRepository<Client>, IClientRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public ClientRepository(DataContext context, IUserHelper userHelper)
            :base(context)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public Client GetClientByUser(User user) //TODO VER SE TEM MAL NAO SER ASYNC
        {
            return _context.Clients.FirstOrDefault(c => c.User == user);
        }

        public Client GetClientWithUser(string username) //TODO VER SE TEM MAL NAO SER ASYNC
        {
            return _context.Clients.Include(c=> c.User).FirstOrDefault(c => c.User.UserName == username);
        }

        public async Task<IQueryable<Client>> GetClientsAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                return null;
            }

            if (await _userHelper.IsUserInRoleAsync(user, RoleNames.Administrator))
            {
                return _context.Clients
                    .Include(c => c.User)
                    .OrderBy(c => c.User.FirstName);
            }

            return _context.Clients
                .Include(c => c.User)
                .Where(c => c.User == user)
                .OrderBy(c => c.User.FirstName);
        }
    }
}
