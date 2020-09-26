using DrPet.Web.Data;
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
using Microsoft.AspNetCore.Mvc.Rendering;
using DrPet.Web.Data.Enums;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace DrPet.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IMailHelpter _mailHelper;
        private readonly IConfiguration _configuration;
        private readonly IClientRepository _clientRepository;        
        private readonly IAdminRepository _adminRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IImageHelper _imageHelper;
        private readonly ISpecializationRepository _specializationRepository;

        public AccountController(IUserHelper userHelper,
            IConverterHelper converterHelper,
            IMailHelpter mailHelper,
            IConfiguration configuration,
            IClientRepository clientRepository,         
            IAdminRepository adminRepository,
            IDoctorRepository doctorRepository,
            IAppointmentRepository appointmentRepository,
            IImageHelper imageHelper,
            ISpecializationRepository specializationRepository)
        {
            _userHelper = userHelper;
            _configuration = configuration;
            _clientRepository = clientRepository;
            _converterHelper = converterHelper;
            _mailHelper = mailHelper;
            _adminRepository = adminRepository;
            _doctorRepository = doctorRepository;
            _appointmentRepository = appointmentRepository;
            _imageHelper = imageHelper;
            _specializationRepository = specializationRepository;
        }

        [HttpPost]
        public async Task<JsonResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);

                if (user != null && !user.IsDeleted)
                {
                    var result = await _userHelper.LoginAsync(model);
                    if (result.Succeeded)
                    {
                        return Json(new { result = "Success" });
                    }
                }           
            }

            return Json(new { result = "Failed" });
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogOutAsync();

            return this.RedirectToAction("Index", "Home");
        }

        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "The email doesn't correspond to a registered user.");
                    return this.View(model);
                }

                var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);

                var link = this.Url.Action(
                    "ResetPassword",
                    "Account",
                    new { token = myToken }, protocol: HttpContext.Request.Scheme);

                _mailHelper.SendMail(model.Email, "Shop Password Reset", $"<h1>Shop Password Reset</h1>" +
                $"To reset the password click in this link:</br></br>" +
                $"<a href = \"{link}\">Reset Password</a>");
                this.ViewBag.Message = "The instructions to recover your password has been sent to email.";
                return this.View();

            }

            return this.View(model);
        }

        public IActionResult ResetPassword(string token)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.UserName);
            if (user != null)
            {
                var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    this.ViewBag.Message = "Password reset successful.";
                    return this.View();
                }

                this.ViewBag.Message = "Error while resetting the password.";
                return View(model);
            }

            this.ViewBag.Message = "User not found.";
            return View(model);
        }

        [Authorize]
        public async Task<JsonResult> UploadImage(IFormCollection form)
        {
            string username = Request.Form["username"]; //TODO VERIFICAR SE ESTE USER É IGUAL AO LOGADO

            var user = await _userHelper.GetUserByEmailAsync(username);

            if (user != null)
            {
                IFormFile image = form.Files[0];

                string path = await _imageHelper.UploadImageAsync(image, "Users");

                user.ImageUrl = path;

                await _userHelper.UpdateUserAsync(user);

                return Json(new { result = "Success" });
            }

            return Json(new { result = "Failed" });
        }

        [Authorize]
        public async Task<IActionResult> Profile(string username) //TODO POR AUTHORIZE NO PROFILE
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            if (username != null && !await _userHelper.IsUserInRoleAsync(user, RoleNames.Client))
            {
                user = await _userHelper.GetUserByEmailAsync(username);
            }

            if (user != null)
            {
                var model = _converterHelper.UserToUserProfileViewModel(user);

                if (await _userHelper.IsUserInRoleAsync(user, RoleNames.Doctor))
                {
                    var doctor = await _doctorRepository.GetDoctorByUserAsync(user);

                    model.Doctor = doctor;
                    //model.Role = RoleNames.Doctor;
                    model.Appointments = await _appointmentRepository.GetDoctorsAppointmentsAsync(user.UserName);
                    model.SpecializationId = doctor.SpecializationId;
                    model.Specializations = _specializationRepository.GetComboSpecializations();
                }
                else
                {
                    model.Appointments = await _appointmentRepository.GetAppointmentsAsync(user.UserName);
                    model.SpecializationId = 1; //SE NAO PASSAR UMA SPECIALIZATIONID DIFERENTE DE 0 O MODELO NÃO SE TORNA VÁLIDO
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
                        model.Doctor.SpecializationId = model.SpecializationId;
                        model.Doctor.Specialization = await _specializationRepository.GetByIdAsync(model.SpecializationId);
                        await _doctorRepository.UpdateAsync(model.Doctor); //SO PARA ATUALIZAR A SPECIALIZATION
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

            return RedirectToAction("Profile", new { username = model.UserName });
        }
    
        public IActionResult Register()
        {
            if (this.User.Identity.IsAuthenticated && !this.User.IsInRole("Admin"))
            {
                return this.RedirectToAction("Profile", "Account");
            }

            var model = new RegisterNewUserViewModel
            {
                Specializations = _specializationRepository.GetComboSpecializations()
            };
            return View(model);
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
                        model.Specializations = _specializationRepository.GetComboSpecializations();
                        return this.View(model);
                    }

                    user = new User //TODO POR ISTO NO CONVERTERHELPER
                    {
                        Email = model.Username,
                        UserName = model.Username,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Gender = model.Gender,
                        DateOfBirth = model.DateOfBirth
                    };

                    var result = await _userHelper.AddUserAsync(user, model.Password);
                    if (result != IdentityResult.Success)
                    {
                        this.ModelState.AddModelError(string.Empty, "The user couldn't be created.");
                        model.Specializations = _specializationRepository.GetComboSpecializations();
                        return this.View(model);
                    }

                    if (model.RoleId <= 0) //TODO VER SE É MELHOR VER SE É <= 0 OU POR NULLABLE
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
                        Roles role = (Roles)model.RoleId;

                        switch (role) //TODO VER SE PONHO ESTE SWITCH NO USERHELPER
                        {
                            case Roles.Admin:
                                await _userHelper.AddUserToRoleAsync(user, RoleNames.Administrator);

                                var admin = new Admin
                                {
                                    User = user
                                };

                                await _adminRepository.CreateAsync(admin);
                                break;
                            case Roles.Client:
                                await _userHelper.AddUserToRoleAsync(user, RoleNames.Client);

                                var client = new Client
                                {
                                    User = user
                                };

                                await _clientRepository.CreateAsync(client);
                                break;
                            case Roles.Doctor:
                                await _userHelper.AddUserToRoleAsync(user, RoleNames.Doctor);

                                var doctor = new Doctor
                                {
                                    User = user,
                                    SpecializationId = model.SpecializationId,
                                    Specialization = await _specializationRepository.GetByIdAsync(model.SpecializationId)
                                };

                                await _doctorRepository.CreateAsync(doctor);
                                break;
                            default:
                                this.ModelState.AddModelError(string.Empty, "The user couldn't be created.");
                                model.Specializations = _specializationRepository.GetComboSpecializations();
                                return this.View(model);
                        }
                    }

                    var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                    var tokenLink = this.Url.Action("ConfirmEmail","Account",new
                    {
                        userid = user.Id,
                        token = myToken,

                    },protocol:HttpContext.Request.Scheme);

                    _mailHelper.SendMail(model.Username, "Email confirmation", $"<h1>Email Confirmation</h1>" +
                      $"To allow the user, " +
                      $"plase click in this link:</br></br><a href = \"{tokenLink}\">Confirm Email</a>");

                    this.ViewBag.Message = "The instructions to allow your user has been sent to email.";

                    return this.View(model);
                }

                this.ModelState.AddModelError(string.Empty, "The user already exists."); //TODO TESTAR COM USER QUE EXISTE PARA VER SE DÁ
            }

            model.Specializations = _specializationRepository.GetComboSpecializations();
            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string userid,string token)
        {
            if (string.IsNullOrEmpty(userid) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByIdAsync(userid);
            if (user ==null)
            {
                return NotFound();
            }

            var result = await _userHelper.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
        }

        [Authorize]
        public async Task<IActionResult> ChangeUser(string username) //TODO FAZER TESTES COM ESTE CHANGE USER PARA TER A CERTEZA QUE ESTOU A PASSAR O USER CORRECTO
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            if (username != null && await _userHelper.IsUserInRoleAsync(user, RoleNames.Administrator))
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
                    model.Doctor =await  _doctorRepository.GetDoctorByUserAsync(user);
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
                    user = _converterHelper.ChangerUserViewModelToUser(model, user); //FIZ ISTO PARA ATUALIZAR O USER COM O QUE ESTA NO MODEL

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
                        return this.RedirectToAction("ChangeUser"); //TODO REDERICIONAR PARA OUTRA PAGINA
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

        //[HttpPost]
        //public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        //{
        //    if (this.ModelState.IsValid)
        //    {
        //        var user = await _userHelper.GetUserByEmailAsync(model.Username);
        //        if (user != null)
        //        {
        //            var result = await _userHelper.ValidatePasswordAsync(
        //                user,
        //                model.Password);

        //            if (result.Succeeded)
        //            {
        //                var claims = new[]
        //                {
        //                     new Claim(JwtRegisteredClaimNames.Sub, user.Email),
        //                     new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        //                 };

        //                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
        //                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //                var token = new JwtSecurityToken(
        //                    _configuration["Tokens:Issuer"],
        //                    _configuration["Tokens:Audience"],
        //                    claims,
        //                    expires: DateTime.UtcNow.AddDays(15),
        //                    signingCredentials: credentials);
        //                var results = new
        //                {
        //                    token = new JwtSecurityTokenHandler().WriteToken(token),
        //                    expiration = token.ValidTo
        //                };

        //                return this.Created(string.Empty, results);
        //            }
        //        }
        //    }

        //    return this.BadRequest();
        //}

        public IActionResult NotAuthorized()
        {
            return View();
        }
    }
}
