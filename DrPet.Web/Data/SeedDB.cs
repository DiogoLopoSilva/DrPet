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
        private readonly IDoctorRepository _doctorRepository;

        public SeedDB(DataContext context, IUserHelper userHelper, IClientRepository clientRepository, IAdminRepository adminRepository, IDoctorRepository doctorRepository)
        {
            _context = context;
            _userHelper = userHelper;
            _clientRepository = clientRepository;
            _adminRepository = adminRepository;
            _doctorRepository = doctorRepository;
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
                    Email = "diogo.lopo.silva@formandos.cinel.pt",
                    UserName = "diogo.lopo.silva@formandos.cinel.pt",
                    FirstName = "Diogo",
                    LastName = "Silva",
                    DateOfBirth = Convert.ToDateTime("25/06/1992"),
                    StreeName = "Praceta das flores nº4",
                    PostalCode = "2685-192",
                    Location = "Loures",
                    PhoneNumber = "916728644",
                    DocumentNumber = "123456789",                   
                };

                var result = await _userHelper.AddUserAsync(user1, "123456");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

                var admin = new Admin
                {
                    User = user1
                };

                await _adminRepository.CreateAsync(admin);

                var isInRole = await _userHelper.IsUserInRoleAsync(user1, RoleNames.Administrator);
                if (!isInRole)
                {
                    await _userHelper.AddUserToRoleAsync(user1, RoleNames.Administrator);
                }
            }

            var user3 = await _userHelper.GetUserByEmailAsync("abc@abc.com");

            if (user3 == null)
            {
                user3 = new User
                {
                    Email = "abc@abc.com",
                    UserName = "abc@abc.com",
                    FirstName = "André",
                    LastName = "Pina",
                    DateOfBirth = Convert.ToDateTime("25/06/1992"),
                    StreeName = "TESTE",
                    PostalCode = "TESTE",
                    Location = "TESTE",
                    PhoneNumber = "123456789",
                    DocumentNumber = "123456789",
                };

                var result = await _userHelper.AddUserAsync(user3, "123456");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

                var doctor = new Doctor
                {
                    User = user3,
                    Specialization = "MESTRE"
                };

                await _doctorRepository.CreateAsync(doctor);

                var isInRole = await _userHelper.IsUserInRoleAsync(user3, RoleNames.Doctor);
                if (!isInRole)
                {
                    await _userHelper.AddUserToRoleAsync(user3, RoleNames.Doctor);
                }
            }

            var user4 = await _userHelper.GetUserByEmailAsync("abc2@abc.com");

            if (user4 == null)
            {
                user4 = new User
                {
                    Email = "abc2@abc.com",
                    UserName = "abc2@abc.com",
                    FirstName = "Sidney",
                    LastName = "Major",
                    DateOfBirth = Convert.ToDateTime("25/06/1992"),
                    StreeName = "TESTE",
                    PostalCode = "TESTE",
                    Location = "TESTE",
                    PhoneNumber = "123456789",
                    DocumentNumber = "123456789",
                };

                var result = await _userHelper.AddUserAsync(user4, "123456");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

                var doctor = new Doctor
                {
                    User = user4,
                    Specialization = "MESTRE 2"
                };

                await _doctorRepository.CreateAsync(doctor);

                var isInRole = await _userHelper.IsUserInRoleAsync(user4, RoleNames.Doctor);
                if (!isInRole)
                {
                    await _userHelper.AddUserToRoleAsync(user4, RoleNames.Doctor);
                }
            }




            var user2 = await _userHelper.GetUserByEmailAsync("diogo.silva_92@hotmail.com");

            if (user2 == null)
            {
                user2 = new User
                {
                    Email = "diogo.silva_92@hotmail.com",
                    UserName = "diogo.silva_92@hotmail.com",
                    FirstName = "DIOGO",
                    LastName = "SILVA",
                    DateOfBirth = Convert.ToDateTime("25/06/1992"),
                    StreeName = "Praceta das flores nº4",
                    PostalCode = "2685-192",
                    Location = "Loures",
                    PhoneNumber = "916728644",
                    DocumentNumber = "123456789",
                };

                var result = await _userHelper.AddUserAsync(user2, "123456");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

                var client = new Client
                {
                    User = user2
                };

                await _clientRepository.CreateAsync(client);

                var isInRole = await _userHelper.IsUserInRoleAsync(user2, RoleNames.Client);
                if (!isInRole)
                {
                    await _userHelper.AddUserToRoleAsync(user2, RoleNames.Client);
                }

                var animal = new Animal
                {
                    Name = "TesteConsulta",
                    Sex = "M",
                    Species = "CAO",
                    Color = "Preto",
                    User = user2
                };

                _context.Animals.Add(animal);

                var appointment = new Appointment
                {
                    Client = client,
                    Animal = animal,
                    Doctor = _doctorRepository.GetDoctorByUser(await _userHelper.GetUserByEmailAsync("abc@abc.com")),
                    Subject="General",
                    StartTime = Convert.ToDateTime($"{DateTime.Today.ToShortDateString()} 09:00"),
                    DoctorNotes = "Teste 1",
                    Status = "Confirmed"
                };

                _context.Appointments.Add(appointment);

                var appointment2 = new Appointment
                {
                    Client = client,
                    Animal = animal,
                    Doctor = _doctorRepository.GetDoctorByUser(await _userHelper.GetUserByEmailAsync("abc@abc.com")),
                    Subject = "General",
                    StartTime = Convert.ToDateTime($"{DateTime.Today.ToShortDateString()} 09:30"),
                    DoctorNotes = "Teste 2",
                    Status = "Waiting Aproval"
                };

                _context.Appointments.Add(appointment2);

                var appointment3 = new Appointment
                {
                    Client = client,
                    Animal = animal,
                    Doctor = _doctorRepository.GetDoctorByUser(await _userHelper.GetUserByEmailAsync("abc2@abc.com")),
                    Subject = "General",
                    StartTime = Convert.ToDateTime($"{DateTime.Today.ToShortDateString()} 09:30"),
                    DoctorNotes = "Teste 3",
                    Status = "Confirmed"
                };

                _context.Appointments.Add(appointment3);
            }

            if (!_context.Animals.Any())
            {
                this.AddProduct("Pipas", "Male", "Bird", "Black", user1);
                this.AddProduct("Whity", "Male", "Dog", "Black", user2);
                this.AddProduct("Xaninha", "Female", "Cat", "Black", user1);
                await _context.SaveChangesAsync();
            }

            //for (int i = 0; i < 50; i++)
            //{
            //    var usertemp = new User
            //    {
            //        Email = $"tempuser{i}@abc.com",
            //        UserName = $"tempuser{i}@abc.com",
            //        FirstName = "TEMP",
            //        LastName = i.ToString(),
            //        DateOfBirth = Convert.ToDateTime("25/06/1992"),
            //        StreeName = "",
            //        PostalCode = "",
            //        Location = "",
            //        PhoneNumber = $"912{i}",
            //        DocumentNumber = "123456789",
            //    };

            //    var result = await _userHelper.AddUserAsync(usertemp, "123456");
            //    if (result != IdentityResult.Success)
            //    {
            //        throw new InvalidOperationException("Could not create the user in seeder");
            //    }

            //    var client = new Client
            //    {
            //        User = usertemp
            //    };

            //    await _clientRepository.CreateAsync(client);

            //    var isInRole = await _userHelper.IsUserInRoleAsync(usertemp, RoleNames.Client);
            //    if (!isInRole)
            //    {
            //        await _userHelper.AddUserToRoleAsync(usertemp, RoleNames.Client);
            //    }
            //}
        }

        private void AddProduct(string name, string sex, string species, string color, User user)
        {
            _context.Animals.Add(new Animal
            {
                Name = name,
                Sex = sex,
                Species = species,
                Color = color,
                User = user
            });
        }
    }
}
