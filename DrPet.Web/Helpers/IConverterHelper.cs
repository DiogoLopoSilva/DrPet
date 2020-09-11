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
        Animal ToAnimal(AnimalViewModel model, string path,bool isNew);

        AnimalViewModel ToAnimalViewModel(Animal animal);

        ChangeUserViewModel UserToChangeUserViewModel(User user);

        User ChangerUserViewModelToUser(ChangeUserViewModel model, User user);
        UserProfileViewModel UserToUserProfileViewModel(User user);
    }
}
