﻿using DrPet.Web.Data.Entities;
using DrPet.Web.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Helpers
{
    public interface IUserHelper
    {
        Task<IdentityResult> AddUserAsync(User user, string password);

        Task<User> GetUserByEmailAsync(string email);

        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogOutAsync();
    }
}
