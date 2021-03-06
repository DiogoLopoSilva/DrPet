﻿using DrPet.Web.Data.Entities;
using DrPet.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        public Animal ToAnimal(AnimalViewModel model, bool isNew)
        {
            return new Animal
            {
                Id = isNew ? 0 : model.Id,
                Name = model.Name,
                Sex = model.Sex,
                Species = model.Species,
                Breed = model.Breed,
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
                DateOfBirth = animal.DateOfBirth,
                User = animal.User
            };
        }

        public AnimalDetailsViewModel ToAnimalDetailsViewModel(Animal animal, IEnumerable<Appointment> appointments)
        {
            return new AnimalDetailsViewModel
            {
                Id = animal.Id,
                Name = animal.Name,
                Sex = animal.Sex,
                Species = animal.Species,
                Breed = animal.Breed,
                DateOfBirth = animal.DateOfBirth,
                ImageUrl = animal.ImageUrl,
                User = animal.User,
                Appointments = appointments
            };
        }

        public Animal AnimalDetailsToAnimal(Animal animal,AnimalDetailsViewModel model)
        {
            animal.Name = model.Name;
            animal.Sex = model.Sex;
            animal.DateOfBirth = model.DateOfBirth;
            animal.Species = model.Species;
            animal.Breed = model.Breed;          

            return animal;              
        }

        public UserProfileViewModel UserToUserProfileViewModel(User user)
        {
            return new UserProfileViewModel
            {
                UserName= user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                StreetName = user.StreeName,
                PostalCode = user.PostalCode,
                Location = user.Location,
                DocumentNumber = user.DocumentNumber,
                PhoneNumber = user.PhoneNumber,
                DateCreated = user.DateCreated,
                ImageUrl = user.ImageUrl
            };
        }

        public User ChangerUserProfileViewModelToUser(UserProfileViewModel model, User user)
        {
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.DateOfBirth = model.DateOfBirth;
            user.StreeName = model.StreetName;
            user.PostalCode = model.PostalCode;
            user.Location = model.Location;
            user.DocumentNumber = model.DocumentNumber;
            user.PhoneNumber = model.PhoneNumber;

            return user;
        }

        //public async Task<ChangeUserViewModel> UserToChangeUserViewModel(User user)
        //{
        //    IList<string> list = await _userManager.GetRolesAsync(user);

        //    if (!list.Any())
        //    {
        //        return null;
        //    }

        //    Human human = null;

        //    switch (list[0])
        //    {
        //        case "Administrator":
        //            human = _adminRepository.GetAdminByUser(user);
        //            break;
        //        case "Client":
        //            human = _clientRepository.GetClientByUser(user);
        //            break;
        //        case "Doctor":
        //            break;
        //        default:
        //            break;
        //    }

        //    if (human == null)
        //    {
        //        return null;
        //    }

        //    return new ChangeUserViewModel { 
        //        FirstName = human.FirstName,
        //        LastName=human.LastName,
        //        DateOfBirth=human.DateOfBirth,
        //        StreeName=human.StreeName,
        //        PostalCode=human.PostalCode,
        //        Location=human.Location,
        //        DocumentNumber=human.DocumentNumber,
        //        Phone=human.Phone                
        //    };
        //}
    }
}
