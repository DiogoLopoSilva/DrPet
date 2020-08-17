using DrPet.Web.Data.Entities;
using DrPet.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data
{
    public class AnimalRepository : GenericRepository<Animal>, IAnimalRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public AnimalRepository(DataContext context, IUserHelper userHelper) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
        }


        public async Task<IQueryable<Animal>> GetAnimalsAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                return null;
            }

            return _context.Animals
                .Where(u => u.User == user)
                .OrderByDescending(o => o.Name);
        }
    }
}
