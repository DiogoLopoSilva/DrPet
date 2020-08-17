using DrPet.Web.Data.Entities;
using DrPet.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        public Animal ToAnimal(AnimalViewModel model, string path, bool isNew)
        {
            return new Animal
            {
                Id = isNew? 0 : model.Id,
                ImageUrl = path,
                Name = model.Name,
                Sex = model.Sex,
                Species = model.Species,
                Breed = model.Breed,
                Color = model.Color,
                DateOfBirth = model.DateOfBirth,
                User = model.User
            };
        }

        public AnimalViewModel ToAnimalViewModel(Animal animal)
        {
            return new AnimalViewModel
            {
                Id = animal.Id,
                ImageUrl = animal.ImageUrl,
                Name = animal.Name,
                Sex = animal.Sex,
                Species = animal.Species,
                Breed = animal.Breed,
                Color = animal.Color,
                DateOfBirth = animal.DateOfBirth,
                User = animal.User
            };
        }
    }
}
