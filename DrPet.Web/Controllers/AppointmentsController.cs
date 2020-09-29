using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DrPet.Web.Data;
using DrPet.Web.Data.Entities;
using DrPet.Web.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using DrPet.Web.Models;
using DrPet.Web.Helpers;

namespace DrPet.Web.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IAnimalRepository _animalRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IClientRepository _clientRepository;
        private readonly ISpecializationRepository _specializationRepository;
        private readonly IMailHelper _mailHelper;

        public AppointmentsController(IUserHelper userHelper,
            IAppointmentRepository appointmentRepository,
            IAnimalRepository animalRepository,
            IDoctorRepository doctorRepository,
            IClientRepository clientRepository,
            ISpecializationRepository specializationRepository,
            IMailHelper mailHelper)
        {
            _userHelper = userHelper;
            _appointmentRepository = appointmentRepository;
            _animalRepository = animalRepository;
            _doctorRepository = doctorRepository;
            _clientRepository = clientRepository;
            _specializationRepository = specializationRepository;
            _mailHelper = mailHelper;
        }

        // GET: Appointments
        public async Task<IActionResult> Index(string username)
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            if (username != null && await _userHelper.IsUserInRoleAsync(user, RoleNames.Administrator))
            {
                user = await _userHelper.GetUserByEmailAsync(username);
            }

            return View(await _appointmentRepository.GetAppointmentsAsync(user.UserName));
        }

        public async Task<ActionResult> TableData(string status, string username)
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            if (username != null && await _userHelper.IsUserInRoleAsync(user, RoleNames.Administrator))
            {
                user = await _userHelper.GetUserByEmailAsync(username);
            }

            if (string.IsNullOrEmpty(status) || status == "All")
            {
                var allitems = await _appointmentRepository.GetAppointmentsAsync(user.UserName);

                return PartialView("_TablePartial", allitems);
            }           

            var items = await _appointmentRepository.GetAppointmentsByStatusAsync(user.UserName,status);

            return PartialView("_TablePartial", items);
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ConfirmAppointment(int Id)
        {
            var appointment = await _appointmentRepository.GetByIdWithModelsAsync(Id);

            if (appointment!=null)
            {
                appointment.Status = "Confirmed";

                await _appointmentRepository.UpdateAsync(appointment);

                try
                {
                    _mailHelper.SendMail(appointment.Client.User.UserName, "Appointment Confirmed", $"<h1>Your appointment has been scheduled!</h1>" +
                      $"Date: {appointment.StartTime}</br></br>" +
                      $"Specialization: {appointment.Specialization.Name}</br></br>" +
                      $"Pet: {appointment.Animal.Name}</br></br>" +
                      $"Doctor: {appointment.Doctor.User.UserName}");
                }
                catch (Exception)
                {
                }

                return Json(new { result = "Success" });
            }

            return Json(new { result = "NotFound" });
        }
      
        public async Task<ActionResult> DeleteAppointment(int Id)
        {
            var appointment = await _appointmentRepository.GetByIdWithModelsAsync(Id);

            if (appointment != null)
            {
                if (appointment.Client.User.UserName != this.User.Identity.Name && !this.User.IsInRole("Admin"))
                {
                    return NotFound();
                }

                await _appointmentRepository.DeleteAsync(appointment);

                try
                {
                    _mailHelper.SendMail(appointment.Client.User.UserName, "Appointment Canceled", $"<h1>Your appointment has been canceled!</h1>" +
                     $"Your appointment for { appointment.Animal.Name} with the date { appointment.StartTime} has been canceled.</br></br>" +
                     $"We are sorry for the inconvenience.");
                }
                catch (Exception)
                {
                }

                var appointments = await _appointmentRepository.GetAppointmentsAsync(this.User.Identity.Name);

                return RedirectToAction("Index", appointments);
            }

            return NotFound();
        }


        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CancelAppointment(int Id)
        {
            var appointment = await _appointmentRepository.GetByIdWithModelsAsync(Id);

            if (appointment != null)
            {
                await _appointmentRepository.DeleteAsync(appointment);

                try
                {
                    _mailHelper.SendMail(appointment.Client.User.UserName, "Appointment not aproved", $"<h1>Your appointment has not been aproved!</h1>" +
                     $"Your appointment for { appointment.Animal.Name} with the date { appointment.StartTime} has not been aproved.</br></br>"+
                     $"Please try scheduling another appointment.");
                }
                catch (Exception)
                {
                }

                return Json(new { result = "Success" });
            }

            return Json(new { result = "NotFound" });
        }

        // GET: Appointments/Details/5
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _appointmentRepository.GetByIdWithModelsAsync(id.Value);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        public async Task<IActionResult> Create(string username)
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            bool isAdmin = await _userHelper.IsUserInRoleAsync(user, RoleNames.Administrator);

            if (String.IsNullOrEmpty(username) && isAdmin)
            {
                return NotFound();
            }

            if (username != null && isAdmin)
            {
                user = await _userHelper.GetUserByEmailAsync(username);
            }

            var model = new AppointmentViewModel
            {
                ClientUsername = user.UserName
            };

            return View(model);
        }

        public async Task<JsonResult> GetData(string username) 
        {

            if (!this.User.IsInRole(RoleNames.Administrator))
            {
                var clientdata = await _appointmentRepository.GetAppointmentsAsync(this.User.Identity.Name);

                foreach (var item in clientdata)
                {
                    if (item.Status=="Confirmed" || item.StartTime.Date <= DateTime.Today.Date)
                    {
                        item.IsReadonly = true;
                    }
                }

                return Json(clientdata);
            }

            var data = _appointmentRepository.GetAllWithModels();

            return Json(data);
        }
        
        public async Task<JsonResult> GetLists(AppointmentViewModel Consulta)
        {
            var doctors = await _doctorRepository.AvailableDoctors(Consulta.StartTime, Consulta.DoctorId, Consulta.SpecializationId);

            if (Consulta.ClientUsername == null)
            {
                return Json(new { result = "Error" });
            }

            var Animals = _animalRepository.GetComboAnimals(Consulta.ClientUsername);

            var Specializations = _specializationRepository.GetComboSpecializationsAppointments();

            return Json(new { result = "Success", list = doctors, listAnimals = Animals, listSpecializations = Specializations });
        }

        public JsonResult GeSpecializationstList(AppointmentViewModel Consulta)
        {
            if (Consulta.ClientUsername == null)
            {
                return Json(new { result = "Error" });
            }

            var Animals = _animalRepository.GetComboAnimals(Consulta.ClientUsername);

            var Specializations = _specializationRepository.GetComboSpecializationsAppointments();

            return Json(new { result = "Success", listSpecializations = Specializations, listAnimals = Animals });
        }

        [HttpPost]
        public async Task<JsonResult> UpdateData([FromBody] EditParams param)
        {
            if (param==null)
            {
                return Json(NotFound());
            }

            if (param.action == "insert" || (param.action == "batch" && param.added.Count != 0)) 
            {
                var value = (param.action == "insert") ? param.value : param.added[0];

                var client = await _clientRepository.GetClientWithUserAsync(value.ClientUsername);

                var doctor = await _doctorRepository.GetDoctorWithUserAsync(value.DoctorId);

                var animal = await _animalRepository.GetAnimalWithUserAsync(value.AnimalId);

                var specialization = await _specializationRepository.GetByIdAsync(value.SpecializationId);

                Appointment appointment = new Appointment
                {
                    Client = client,
                    Animal = animal,
                    Doctor = doctor,
                    Specialization = specialization,
                    Status = "Waiting",
                    StartTime = value.StartTime,
                    ClientDescription = value.ClientDescription
                };

                await _appointmentRepository.CreateAsync(appointment);

                try
                {
                    _mailHelper.SendMail(appointment.Client.User.UserName, "Appointment Requested", $"<h1>You have requested an appointment</h1>" +
                      $"Please wait while we analyze your request!");
                }
                catch (Exception)
                {
                }
          
            }
            if (param.action == "update" || (param.action == "batch" && param.changed.Count != 0))
            {
                var value = param.changed[0];

                var appointment = await _appointmentRepository.GetByIdWithModelsAsync(value.Id);

                if (appointment!=null)
                {
                    appointment.Animal = await _animalRepository.GetAnimalWithUserAsync(value.AnimalId);

                    appointment.Doctor = await _doctorRepository.GetDoctorWithUserAsync(value.DoctorId);

                    appointment.Specialization = await _specializationRepository.GetByIdAsync(value.SpecializationId);

                    appointment.ClientDescription = value.ClientDescription;

                    await _appointmentRepository.UpdateAsync(appointment);

                    try
                    {
                        _mailHelper.SendMail(appointment.Client.User.UserName, "Appointment Updated", $"<h1>Your Appointment request has been updated.</h1>" +
                          $"Please wait while we analyze your request!");
                    }
                    catch (Exception)
                    {
                    }
                }             

            }
            if (param.action == "remove" || (param.action == "batch" && param.deleted.Count != 0))
            {
                if (param.action == "remove")
                {

                }
                else
                {
                    foreach (var apps in param.deleted)
                    {
                        var appointment = await _appointmentRepository.GetByIdWithModelsAsync(apps.Id);

                        if (appointment!=null && appointment.Status=="Waiting")
                        {
                           await _appointmentRepository.DeleteAsync(appointment);

                            try
                            {
                                _mailHelper.SendMail(appointment.Client.User.UserName, "Appointment Canceled", $"<h1>Your appointment request has been canceled!</h1>" +
                                 $"Your appointment for { appointment.Animal.Name} with the date { appointment.StartTime} has been canceled.");
                            }
                            catch (Exception)
                            {
                            }
                        }

                    }
                }

            }

            var data = _appointmentRepository.GetAllWithModels().ToList();

            return Json(data, new Newtonsoft.Json.JsonSerializerSettings());
        }

        public async Task<JsonResult> StartAppointment(int? id) 
        {

            if (id==null)
            {
                return Json(new { result = "Error" });
            }

            var appointment = await _appointmentRepository.GetByIdAsync(id.Value);

            if (appointment==null)
            {
                return Json(new { result = "Error" });
            }

            appointment.Status = "InProgress";

            await _appointmentRepository.UpdateAsync(appointment);
           
            return Json(new { result = "Success"});
        }

        public async Task<JsonResult> EndAppointment(int? id,string notes)  
        {

            if (id == null)
            {
                return Json(new { result = "Error" });
            }

            var appointment = await _appointmentRepository.GetByIdAsync(id.Value);

            if (appointment == null)
            {
                return Json(new { result = "Error" });
            }

            appointment.Status = "Completed";
            appointment.DoctorNotes = notes;

            await _appointmentRepository.UpdateAsync(appointment);

            return Json(new { result = "Success" });
        }
    }
}
