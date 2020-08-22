using DrPet.Web.Data.Entities;
using DrPet.Web.Helpers;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DrPet.Web.Data.Repositories;

namespace DrPet.Web.Data
{
    public class SeedDB
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IClientRepository _clientRepository;
        private readonly IAdminRepository _adminRepository;

        public SeedDB(DataContext context, IUserHelper userHelper, IClientRepository clientRepository, IAdminRepository adminRepository)
        {
            _context = context;
            _userHelper = userHelper;
            _clientRepository = clientRepository;
            _adminRepository = adminRepository;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            await _userHelper.CheckRoleAsync(RoleNames.Administrator);
            await _userHelper.CheckRoleAsync(RoleNames.Client);
            await _userHelper.CheckRoleAsync(RoleNames.Doctor);

            var user1 = await _userHelper.GetUserByEmailAsync("diogo.lopo.silva@formandos.cinel.pt");

            if (user1 == null)
            {
                user1 = new User
                {
                    Email= "diogo.lopo.silva@formandos.cinel.pt",
                    UserName= "diogo.lopo.silva@formandos.cinel.pt",
                    FirstName = "Diogo",
                    LastName = "Silva",
                    DateOfBirth = Convert.ToDateTime("25/06/1992"),
                    StreeName = "Praceta das flores nº4",
                    PostalCode = "2685-192",
                    Location = "Loures",
                    Phone = "916728644",
                    DocumentNumber = "123456789",
                };

                var result = await _userHelper.AddUserAsync(user1, "123456");
                if (result!=IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

                var admin = new Admin {
                User = user1
                };

                await _adminRepository.CreateAsync(admin);
            }

            var isInRole = await _userHelper.IsUserInRoleAsync(user1, RoleNames.Administrator);
            if (!isInRole)
            {
                await _userHelper.AddUserToRoleAsync(user1, RoleNames.Administrator);
            }

            var user2 = await _userHelper.GetUserByEmailAsync("diogo.silva_92@hotmail.com");

            if (user2 == null)
            {
                user2 = new User
                {
                    Email = "diogo.silva_92@hotmail.com",
                    UserName = "diogo.silva_92@hotmail.com",
                    FirstName = "Diogo",
                    LastName = "Silva",
                    DateOfBirth = Convert.ToDateTime("25/06/1992"),
                    StreeName = "Praceta das flores nº4",
                    PostalCode = "2685-192",
                    Location = "Loures",
                    Phone = "916728644",
                    DocumentNumber = "123456789",
                };

                var result = await _userHelper.AddUserAsync(user2, "654321");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

                var client = new Client
                {                   
                    User = user2
                };

                await _clientRepository.CreateAsync(client);
            }

            isInRole = await _userHelper.IsUserInRoleAsync(user2, RoleNames.Client);
            if (!isInRole)
            {
                await _userHelper.AddUserToRoleAsync(user2, RoleNames.Client);
            }

            if (!_context.Animals.Any())
            {
                this.AddProduct("Pipas", "Male", "Bird", "Black", user1);
                this.AddProduct("Whity","Male", "Dog", "Black", user2);
                this.AddProduct("Xaninha","Female", "Cat", "Black", user1);
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
