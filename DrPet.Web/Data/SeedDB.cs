using DrPet.Web.Data.Entities;
using DrPet.Web.Helpers;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data
{
    public class SeedDB
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public SeedDB(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            var user = await _userHelper.GetUserByEmailAsync("diogo.lopo.silva@formandos.cinel.pt");

            if (user == null)
            {
                user = new User
                {
                    FirstName="Diogo",
                    LastName="Silva",
                    Email= "diogo.lopo.silva@formandos.cinel.pt",
                    UserName= "diogo.lopo.silva@formandos.cinel.pt",
                    PhoneNumber="911859473"
                };

                var result = await _userHelper.AddUserAsync(user, "123456");
                if (result!=IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }
            }

            if (!_context.Animals.Any())
            {
                this.AddProduct("Pipas", "Male", "Bird", "Black", user);
                this.AddProduct("Whity","Male", "Dog", "Black", user);
                this.AddProduct("Xaninha","Female", "Cat", "Black", user);
                await _context.SaveChangesAsync();
            }
        }

        private void AddProduct(string name, string sex, string species, string color, User user)
        {
            _context.Animals.Add(new Animal
            {
                Name=name,
                Sex=sex,
                Species=species,
                Color=color,
                User = user
            });
        }
    }    
}
