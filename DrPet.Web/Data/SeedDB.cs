using DrPet.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data
{
    public class SeedDB
    {
        private readonly DataContext _context;

        public SeedDB(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            if (!_context.Animals.Any())
            {
                this.AddProduct("Pipas", "Male", "Bird", "Black");
                this.AddProduct("Whity","Male", "Dog", "Black");
                this.AddProduct("Xaninha","Female", "Cat", "Black");
                await _context.SaveChangesAsync();
            }
        }

        private void AddProduct(string name, string sex, string species, string color)
        {
            _context.Animals.Add(new Animal
            {
                Name=name,
                Sex=sex,
                Species=species,
                Color=color
            });
        }
    }    
}
