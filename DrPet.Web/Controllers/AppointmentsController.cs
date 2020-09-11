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
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IAnimalRepository _animalRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IClientRepository _clientRepository;

        public AppointmentsController(DataContext context,
            IUserHelper userHelper,
            IAppointmentRepository appointmentRepository,
            IAnimalRepository animalRepository,
            IDoctorRepository doctorRepository,
            IClientRepository clientRepository)
        {
            _context = context;
            _userHelper = userHelper;
            _appointmentRepository = appointmentRepository;
            _animalRepository = animalRepository;
            _doctorRepository = doctorRepository;
            _clientRepository = clientRepository;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            //var model = new AppointmentsWithTempView
            //{
            //    Appointments = await _appointmentRepository.GetAppointmentsAsync(this.User.Identity.Name),
            //    AppointmentsTemp = await _appointmentRepository.GetAppointmentsTempAsync(this.User.Identity.Name)
            //};

            return View(await _appointmentRepository.GetAppointmentsAsync(this.User.Identity.Name));         
        }

        // GET: AppointmentsNotConfirmed
        public async Task<IActionResult> IndexNotConfirmed()
        {
            return View(await _appointmentRepository.GetUnconfirmedAppointmentsAsync(this.User.Identity.Name));
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        public async Task<IActionResult> Create(string username) //TODO VERIFICAR QUE NAO TEM BUGS
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            bool isAdmin = await _userHelper.IsUserInRoleAsync(user, RoleNames.Administrator);

            if(String.IsNullOrEmpty(username) && isAdmin)
            {
                return NotFound();
            }

            if (username != null && isAdmin)
            {
                user = await _userHelper.GetUserByEmailAsync(username);
            }

            var model = new AppointmentViewModel
            {
                //Animals = _animalRepository.GetComboAnimals(user.UserName),
                ClientUsername = user.UserName,
                //Date = DateTime.Today, //Para começar com a data currente
                //Appointments = _appointmentRepository.GetAllWithModels()
            };

            //ViewBag.appointments = GetScheduleData();
            return View(model);
        }

        public JsonResult GetData(string username)  // Here we get the Start and End Date and based on that can filter the data and return to Scheduler --VER Passing additional parameters to the server
        {
            var data = _appointmentRepository.GetAllWithModels().ToList();

            foreach (var item in data)
            {
                if (item.Client.User.UserName != username)
                {
                    item.IsReadonly = true;
                    item.Subject = "Date not available";
                }
                else
                {
                    item.Subject = "Your appointment";
                }
            }

            return Json(data);
        }

        [HttpPost]
        public JsonResult GetLists(AppointmentViewModel Consulta)  // Here we get the Start and End Date and based on that can filter the data and return to Scheduler --VER Passing additional parameters to the server
        {
            var doctors = _doctorRepository.AvailableDoctors(Consulta.StartTime, Consulta.DoctorId);

            if (Consulta.ClientUsername == null)
            {
                return Json(new { result = "Error" });
            }

            var Animals = _animalRepository.GetComboAnimals(Consulta.ClientUsername);

            return Json(new { result = "Success", list = doctors, listAnimals = Animals });
        }

        [HttpPost]
        public async Task<JsonResult> UpdateData([FromBody]EditParams param)
        {
            if (param.action == "insert" || (param.action == "batch" && param.added != null)) // this block of code will execute while inserting the appointments
            {
                var value = (param.action == "insert") ? param.value : param.added[0];


                var client = _clientRepository.GetClientWithUser(value.ClientUsername);

                var doctor =  _doctorRepository.GetDoctorWithUser(value.DoctorId);

                var animal = _animalRepository.GetAnimalWithUser(value.AnimalId);

                Appointment appointment = new Appointment()
                {
                    Client = client,
                    Animal = animal,
                    Doctor = doctor,
                    Subject = "POR ALGO AQUI",
                    Status = "Waiting Aproval",
                    StartTime = value.StartTime.ToLocalTime(),
                    ClientDescription = value.Description
                };

                await _appointmentRepository.CreateAsync(appointment);

                var data = _appointmentRepository.GetAllWithModels().ToList();

                foreach (var item in data)
                {
                    //item.IsReadonly = true;
                    item.Subject = "Not available";
                }

                return Json(data, new Newtonsoft.Json.JsonSerializerSettings());
            }
            if (param.action == "update" || (param.action == "batch" && param.changed != null)) // this block of code will execute while updating the appointment
            {
                
            }
            if (param.action == "remove" || (param.action == "batch" && param.deleted != null)) // this block of code will execute while removing the appointment
            {
                if (param.action == "remove")
                {
                    
                }
                else
                {
                    foreach (var apps in param.deleted)
                    {
                        
                    }
                }
               
            }

            return Json(new { result = "Failed" });
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.ClientUsername);

                if (user == null)
                {
                    return NotFound();
                }

                var client = _clientRepository.GetClientByUser(user);

                var animal = _animalRepository.GetAnimalWithUser(model.AnimalId);

                var doctor = _doctorRepository.GetDoctorWithUser(model.DoctorId);
                //TODO VERIFICAÇAOES SE USER EXISTE ETC ETC

                //var appointmentTemp = new AppointmentTemp
                //{
                //    Client = client,
                //    Animal= animal,
                //    Doctor = doctor,
                //    Date = model.Date,
                //    Notes = model.Notes
                //};

                //await _appointmentRepository.CreateAppointmentTemp(appointmentTemp);

                return RedirectToAction("IndexTemp");
            }
            return View(model);
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Notes")] Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(appointment);
        }

        public IActionResult DeleteAppointmentTemp(int? id)
        {
            if (id == null)
            {
                return NotFound(); //TODO REDIRECIONAR
            }

            //await _appointmentRepository.DeleteAppointmentTempAsync(id.Value);
            return this.RedirectToAction("IndexTemp");
        }

        //[Authorize(Roles = RoleNames.Administrator)]
        //public async Task<IActionResult> ConfirmAppointmentTemp(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound(); //TODO REDIRECIONAR
        //    }

        //    var success = await _appointmentRepository.ConfirmAppointmentAsync(id.Value);

        //    if (success)
        //    {
        //        return this.RedirectToAction(nameof(Index));
        //    }

        //    return this.RedirectToAction("IndexTemp");
        //}

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}
