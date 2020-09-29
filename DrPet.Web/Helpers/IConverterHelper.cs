using DrPet.Web.Data.Entities;
using DrPet.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Helpers
{
    public interface IConverterHelper
    {
        Animal ToAnimal(AnimalViewModel model,bool isNew);

        AnimalViewModel ToAnimalViewModel(Animal animal);

        UserProfileViewModel UserToUserProfileViewModel(User user);

        User ChangerUserProfileViewModelToUser(UserProfileViewModel model, User user);

        AnimalDetailsViewModel ToAnimalDetailsViewModel(Animal animal, IEnumerable<Appointment> appointments);

        Animal AnimalDetailsToAnimal(Animal animal,AnimalDetailsViewModel model);
    }
}
