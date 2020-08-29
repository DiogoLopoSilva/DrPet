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

        public async Task<IActionResult> IndexTemp()
        {
            //var model = new AppointmentsWithTempView
            //{
            //    Appointments = await _appointmentRepository.GetAppointmentsAsync(this.User.Identity.Name),
            //    AppointmentsTemp = await _appointmentRepository.GetAppointmentsTempAsync(this.User.Identity.Name)
            //};

            return View(await _appointmentRepository.GetAppointmentsTempAsync(this.User.Identity.Name));
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

            if (username != null && await _userHelper.IsUserInRoleAsync(user, RoleNames.Administrator))
            {
                user = await _userHelper.GetUserByEmailAsync(username);
            }

            var model = new AppointmentViewModel
            {
                Animals = _animalRepository.GetComboAnimals(user.UserName),
                Doctors = _doctorRepository.GetComboDoctors(),
                User = user
            };

            return View(model);
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
                var user = await _userHelper.GetUserByEmailAsync(model.User.UserName);

                if (user == null)
                {
                    return NotFound();
                }

                var client = _clientRepository.GetClientByUser(user);

                var animal = _animalRepository.GetAnimalWithUser(model.AnimalId);

                var doctor = _doctorRepository.GetDoctorWithUser(model.DoctorId);
                //TODO VERIFICAÇAOES SE USER EXISTE ETC ETC

                var appointmentTemp = new AppointmentTemp
                {
                    Client = client,
                    Animal= animal,
                    Doctor = doctor,
                    Date = model.Date,
                    Notes = model.Notes
                };

                await _appointmentRepository.CreateAppointmentTemp(appointmentTemp);

                return RedirectToAction(nameof(Index));
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

        public async Task<IActionResult> DeleteAppointmentTemp(int? id)
        {
            if (id == null)
            {
                return NotFound(); //TODO REDIRECIONAR
            }

            await _appointmentRepository.DeleteAppointmentTempAsync(id.Value);
            return this.RedirectToAction("IndexTemp");
        }

        [Authorize(Roles = RoleNames.Administrator)]
        public async Task<IActionResult> ConfirmAppointmentTemp(int? id)
        {
            if (id == null)
            {
                return NotFound(); //TODO REDIRECIONAR
            }

            var success = await _appointmentRepository.ConfirmAppointmentAsync(id.Value);

            if (success)
            {
                return this.RedirectToAction(nameof(Index));
            }

            return this.RedirectToAction("IndexTemp");
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}
