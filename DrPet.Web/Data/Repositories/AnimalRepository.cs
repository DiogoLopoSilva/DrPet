using DrPet.Web.Data.Entities;
using DrPet.Web.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Repositories
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

            if (await _userHelper.IsUserInRoleAsync(user, RoleNames.Administrator))
            {
                return _context.Animals
                    .Include(c => c.User)
                    .OrderBy(c => c.User.FirstName);
            }

            return _context.Animals
                .Include(c => c.User)
                .Where(c => c.User == user)
                .OrderBy(c => c.User.FirstName);
        }

        public Animal GetAnimalWithUser(int id)
        {
            return _context.Animals.Include(a => a.User).FirstOrDefault(a => a.Id == id);
        }

        public IEnumerable<SelectListItem> GetComboAnimals(string userName)
        {
            var list = _context.Animals.Where(a => a.User.UserName == userName).Select(a => new SelectListItem
            {
                Text = a.Name,
                Value = a.Id.ToString()
            }).ToList();

            //list.Insert(0, new SelectListItem
            //{
            //    Text = ("Select a animal..."),
            //    Value = "0"
            //});

            return list;
        }
    }
}
