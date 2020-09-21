﻿using DrPet.Web.Data;
using DrPet.Web.Data.Entities;
using DrPet.Web.Helpers;
using DrPet.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DrPet.Web.Data.Repositories;
using Microsoft.AspNetCore.Http;

namespace DrPet.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IClientRepository _clientRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IAdminRepository _adminRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IImageHelper _imageHelper;

        public AccountController(IUserHelper userHelper,
            IClientRepository clientRepository,
            IConverterHelper converterHelper,
            IAdminRepository adminRepository,
            IDoctorRepository doctorRepository,
            IAppointmentRepository appointmentRepository,
            IImageHelper imageHelper)
        {
            _userHelper = userHelper;
            _clientRepository = clientRepository;
            _converterHelper = converterHelper;
            _adminRepository = adminRepository;
            _doctorRepository = doctorRepository;
            _appointmentRepository = appointmentRepository;
            _imageHelper = imageHelper;
        }

        //public IActionResult Login()
        //{
        //    if (this.User.Identity.IsAuthenticated)
        //    {
        //        return this.RedirectToAction("Index", "Home");
        //    }

        //    return PartialView("_Login");

        //    //return this.View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> Login(LoginViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var result = await _userHelper.LoginAsync(model);
        //        if (result.Succeeded)
        //        {
        //            if (this.Request.Query.Keys.Contains("ReturnUrl"))
        //            {
        //                //Direção de retorno
        //                return Redirect(this.Request.Query["ReturnUrl"].First());
        //            }

        //            return this.RedirectToAction("Index", "Home");
        //        }
        //    }

        //    this.ModelState.AddModelError(string.Empty, "Failed to login.");

        //    //return View(model);
        //    return PartialView("_Login", model);
        //}

        [HttpPost]
        public async Task<JsonResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userHelper.LoginAsync(model);
                if (result.Succeeded)
                {
                    return Json(new { result = "Success" });
                }
            }

            return Json(new { result = "Failed" });
        }

        public async Task<JsonResult> UploadImage(IFormCollection form)
        {          
            string username = Request.Form["username"];

            var user = await _userHelper.GetUserByEmailAsync(username);

            if (user!=null)
            {
                IFormFile image = form.Files[0];

                string path = await _imageHelper.UploadImageAsync(image, "Users");

                user.ImageUrl = path;

                await _userHelper.UpdateUserAsync(user);

                return Json(new { result = "Success" });
            }            

            return Json(new { result = "Failed" });
        }

        public async Task<IActionResult> Profile(string username) //TODO POR AUTHORIZE NO PROFILE
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            if (username != null && await _userHelper.IsUserInRoleAsync(user, RoleNames.Administrator))
            {
                user = await _userHelper.GetUserByEmailAsync(username);
            }

            if (user != null)
            {
               var model = _converterHelper.UserToUserProfileViewModel(user);

                if (await _userHelper.IsUserInRoleAsync(user, RoleNames.Doctor))
                {
                    model.Doctor = _doctorRepository.GetDoctorByUser(user);
                    model.Role = RoleNames.Doctor;
                    model.Appointments = await _appointmentRepository.GetDoctorsAppointmentsAsync(user.UserName);
                }
                else
                {
                    model.Appointments = await _appointmentRepository.GetAppointmentsAsync(user.UserName);
                }

                return View(model);
            };

            return NotFound(); //TODO RETURN NOT FOUND
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(UserProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                if ((this.User.Identity.Name != model.UserName) && !this.User.IsInRole("Admin"))
                {
                    return NotFound();
                }

                var user = await _userHelper.GetUserByEmailAsync(model.UserName);
                if (user != null)
                {
                    user = _converterHelper.ChangerUserProfileViewModelToUser(model, user); //FIZ ISTO PARA ATUALIZAR O USER COM O QUE ESTA NO MODEL

                    var response = await _userHelper.UpdateUserAsync(user);

                    if (model.Doctor != null)
                    {
                        await _doctorRepository.UpdateAsync(model.Doctor);
                    }

                    if (response.Succeeded)
                    {
                        ViewBag.UserMessage = "User update!";
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "user not found!");
                }
            }

            return RedirectToAction("Profile", new { username = model.UserName});
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogOutAsync();

            return this.RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            if (this.User.Identity.IsAuthenticated && !this.User.IsInRole("Admin"))
            {
                return this.RedirectToAction("Profile", "Account");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterNewUserViewModel model) //TODO FAZER TESTES COM O NOVO REGISTER
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user == null)
                {
                    if (model.RoleId >= 1 && !this.User.IsInRole("Admin"))
                    {
                        this.ModelState.AddModelError(string.Empty, "Stop trying to hack.");
                        return this.View(model);
                    }

                    user = new User
                    {                       
                        Email = model.Username,
                        UserName = model.Username,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                    };

                    var result = await _userHelper.AddUserAsync(user, model.Password);
                    if (result != IdentityResult.Success)
                    {
                        this.ModelState.AddModelError(string.Empty, "The user coudln't be created.");
                        return this.View(model);
                    }

                    if (model.RoleId<=0) //TODO VER SE É MELHOR VER SE É <= 0 OU POR NULLABLE
                    {
                        await _userHelper.AddUserToRoleAsync(user, RoleNames.Client);

                        var client = new Client
                        {
                            User = user
                        };

                        await _clientRepository.CreateAsync(client);
                    }
                    else
                    {
                        RolesEnum role = (RolesEnum)model.RoleId;

                        switch (role)
                        {
                            case RolesEnum.Admin:
                                await _userHelper.AddUserToRoleAsync(user, RoleNames.Administrator);

                                var admin = new Admin
                                {
                                    User = user
                                };

                                await _adminRepository.CreateAsync(admin);
                                break;
                            case RolesEnum.Client:
                                await _userHelper.AddUserToRoleAsync(user, RoleNames.Client);

                                var client = new Client
                                {
                                    User = user
                                };

                                await _clientRepository.CreateAsync(client);
                                break;
                            case RolesEnum.Doctor:
                                await _userHelper.AddUserToRoleAsync(user, RoleNames.Doctor);

                                var doctor = new Doctor
                                {
                                    User = user
                                };

                                await _doctorRepository.CreateAsync(doctor);
                                break;
                            default:
                                this.ModelState.AddModelError(string.Empty, "The user couldn't be created.");
                                return this.View(model);
                        }
                    }                                  

                    var loginViewModel = new LoginViewModel
                    {
                        Password = model.Password,
                        RememberMe = false,
                        Username = model.Username
                    };

                    var result2 = await _userHelper.LoginAsync(loginViewModel);

                    if (result2.Succeeded)
                    {
                        return this.RedirectToAction("Index", "Home");
                    }

                    this.ModelState.AddModelError(string.Empty, "The user couldn't login.");
                    return this.View(model);
                }

                this.ModelState.AddModelError(string.Empty, "The user already exists.");
                return this.View(model);
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult RegisterAdmin()
        {
            return View("Register");
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAdmin(RegisterNewUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user == null)
                {
                    user = new User
                    {
                        Email = model.Username,
                        UserName = model.Username,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                    };

                    var result = await _userHelper.AddUserAsync(user, model.Password);
                    if (result != IdentityResult.Success)
                    {
                        if (result.Errors.Any(e=> e.Description.Contains("Password")))
                        {
                            this.ModelState.AddModelError(string.Empty, "Password must contain 1-6 chars");
                            return this.View(model);
                        }

                        this.ModelState.AddModelError(string.Empty, "The user coudln't be created.");
                        return this.View(model);
                    }

                    await _userHelper.AddUserToRoleAsync(user, RoleNames.Administrator);

                    var admin = new Admin
                    {
                        User=user
                    };

                    await _adminRepository.CreateAsync(admin);

                    var loginViewModel = new LoginViewModel
                    {
                        Password = model.Password,
                        RememberMe = false,
                        Username = model.Username
                    };

                    var result2 = await _userHelper.LoginAsync(loginViewModel);

                    if (result2.Succeeded)
                    {
                        return this.RedirectToAction("Index", "Home");
                    }

                    this.ModelState.AddModelError(string.Empty, "The user coudln't login.");
                    return this.View(model);
                }

                this.ModelState.AddModelError(string.Empty, "The user already exists.");
                return this.View(model);
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult RegisterDoctor()
        {
            return View("Register");
        }

        [HttpPost]
        public async Task<IActionResult> RegisterDoctor(RegisterNewUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user == null)
                {
                    user = new User
                    {
                        Email = model.Username,
                        UserName = model.Username,
                        FirstName = model.FirstName,
                        LastName = model.LastName
                    };

                    var result = await _userHelper.AddUserAsync(user, model.Password);
                    if (result != IdentityResult.Success)
                    {
                        this.ModelState.AddModelError(string.Empty, "The user coudln't be created.");
                        return this.View(model);
                    }

                    await _userHelper.AddUserToRoleAsync(user, RoleNames.Doctor);

                    var doctor = new Doctor
                    {
                       User = user
                    };

                    await _doctorRepository.CreateAsync(doctor);

                    var loginViewModel = new LoginViewModel
                    {
                        Password = model.Password,
                        RememberMe = false,
                        Username = model.Username
                    };

                    var result2 = await _userHelper.LoginAsync(loginViewModel);

                    if (result2.Succeeded)
                    {
                        return this.RedirectToAction("Index", "Home");
                    }

                    this.ModelState.AddModelError(string.Empty, "The user coudln't login.");
                    return this.View(model);
                }

                this.ModelState.AddModelError(string.Empty, "The user already exists.");
                return this.View(model);
            }

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> ChangeUser(string username) //TODO FAZER TESTES COM ESTE CHANGE USER PARA TER A CERTEZA QUE ESTOU A PASSAR O USER CORRECTO
        {
            //var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            //var model = new MultipleViewModel();

            //model.C = new ChangeUserViewModel();


            //if (user != null)
            //{
            //    model.C = _converterHelper.UserToChangeUserViewModel(user);
            //};

            //if (await _userHelper.IsUserInRoleAsync(user,RoleNames.Doctor))
            //{
            //    model.D =  _doctorRepository.GetDoctorByUser(user);
            //}

            //return View(model);

            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            if (username != null && await _userHelper.IsUserInRoleAsync(user,RoleNames.Administrator))
            {
                user = await _userHelper.GetUserByEmailAsync(username);
            }          

            var model = new ChangeUserViewModel();


            if (user != null)
            {
                model = _converterHelper.UserToChangeUserViewModel(user);

                model.User = user;

                if (await _userHelper.IsUserInRoleAsync(user, RoleNames.Doctor))
                {
                    model.Doctor = _doctorRepository.GetDoctorByUser(user);
                }
            };

            return View(model); //TODO RETURN NOT FOUND
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUser(ChangeUserViewModel model)
        {
           if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.User.UserName);
                if (user != null)
                {
                    user = _converterHelper.ChangerUserViewModelToUser(model,user); //FIZ ISTO PARA ATUALIZAR O USER COM O QUE ESTA NO MODEL

                    var response = await _userHelper.UpdateUserAsync(user);

                    if (model.Doctor !=null)
                    {
                        await _doctorRepository.UpdateAsync(model.Doctor);
                    }

                    if (response.Succeeded)
                    {
                        ViewBag.UserMessage = "User update!";
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "user not found!");
                }
            }

            return View(model);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                if (user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return this.RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        this.ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "User no found.");
                }
            }

            return View(model);
        }

        public IActionResult ShowModal()
        {
            //specify the name or path of the partial view
            return PartialView("_Login");
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }
    }
}
