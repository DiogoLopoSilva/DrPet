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

        public async Task<Client> GetClientByUserAsync(User user) //TODO VER SE TEM MAL NAO SER ASYNC
        {
            return await _context.Clients.Include(c => c.User).FirstOrDefaultAsync(c => c.User == user);
        }

        public async Task<Client> GetClientWithUserAsync(string username) //TODO VER SE TEM MAL NAO SER ASYNC
        {
            return await _context.Clients.Include(c=> c.User).FirstOrDefaultAsync(c => c.User.UserName == username);
        }

        public IQueryable<Client> GetClients()
        {
            return _context.Clients
                     .Include(c => c.User).Where(d => d.User.EmailConfirmed && !d.IsDeleted)
                     .OrderBy(c => c.User.FirstName);
        }

        public async Task DeleteClientWithUser(Client client)
        {
            var appointments = _context.Appointments.Where(a => a.Client == client && a.Status != "Completed");

            var animals = _context.Animals.Where(a => a.User == client.User);

            _context.Appointments.RemoveRange(appointments);

            _context.Animals.RemoveRange(animals);

            _context.Clients.Remove(client);
            _context.Users.Remove(client.User);

            await _context.SaveChangesAsync();
        }
    }
}
