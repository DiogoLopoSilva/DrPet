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
        private readonly ISpecializationRepository _specializationRepository;

        public AppointmentsController(DataContext context,
            IUserHelper userHelper,
            IAppointmentRepository appointmentRepository,
            IAnimalRepository animalRepository,
            IDoctorRepository doctorRepository,
            IClientRepository clientRepository,
            ISpecializationRepository specializationRepository)
        {
            _context = context;
            _userHelper = userHelper;
            _appointmentRepository = appointmentRepository;
            _animalRepository = animalRepository;
            _doctorRepository = doctorRepository;
            _clientRepository = clientRepository;
            _specializationRepository = specializationRepository;
        }

        // GET: Appointments
        public async Task<IActionResult> Index(string username)
        {
            //var model = new AppointmentsWithTempView
            //{
            //    Appointments = await _appointmentRepository.GetAppointmentsAsync(this.User.Identity.Name),
            //    AppointmentsTemp = await _appointmentRepository.GetAppointmentsTempAsync(this.User.Identity.Name)
            //};

            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            if (username != null && await _userHelper.IsUserInRoleAsync(user, RoleNames.Administrator))
            {
                user = await _userHelper.GetUserByEmailAsync(username);
            }

            //var model = new AppointmentIndexViewModel
            //{
            //    CompleteList = await _appointmentRepository.GetAppointmentsAsync(user.UserName)
            //};

            //var model = new AppointmentIndexViewModel(await _appointmentRepository.GetAppointmentsAsync(user.UserName));


            return View(await _appointmentRepository.GetAppointmentsAsync(user.UserName));
        }

        public async Task<ActionResult> TableData(string status, string username) //TODO FAZER UM GET APPOINTMENTS BY STATUS
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            if (username != null && await _userHelper.IsUserInRoleAsync(user, RoleNames.Administrator))
            {
                user = await _userHelper.GetUserByEmailAsync(username);
            }

            if (string.IsNullOrEmpty(status) || status == "All") // TENTAR AMANHA FAZER UM METODO JSON QUE DEVOLVE A LISTA DE DATA E UM PartialView("_TablePartial", allitems);
            {
                var allitems = await _appointmentRepository.GetAppointmentsAsync(user.UserName);

                //return Json(new { result = "Success", list = allitems });

                return PartialView("_TablePartial", allitems);
            }           

            var items = await _appointmentRepository.GetAppointmentsByStatusAsync(user.UserName,status);

            //return Json(new { result = "Success", list = items });

            return PartialView("_TablePartial", items); //TODO SE O .HTML NAO DER, USAR .REPLACE
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ConfirmAppointment(int Id) //TODO FAZER UM GET APPOINTMENTS BY STATUS
        {
            var appointment = _appointmentRepository.GetByIdWithModels(Id);

            if (appointment!=null)
            {
                appointment.Status = "Confirmed";

                await _appointmentRepository.UpdateAsync(appointment);

                return Json(new { result = "Success" });
            }

            return Json(new { result = "NotFound" });
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CancelAppointment(int Id) //TODO FAZER UM GET APPOINTMENTS BY STATUS
        {
            var appointment = _appointmentRepository.GetByIdWithModels(Id);

            if (appointment != null)
            {
                await _appointmentRepository.DeleteAsync(appointment);

                return Json(new { result = "Success" });
            }

            return Json(new { result = "NotFound" });
        }

        public IActionResult TableView(string JsonList)
        {
            var listAppointment = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Appointment>>(JsonList);

            return PartialView("_TablePartial",listAppointment);
        }

        // GET: Appointments/Details/5
        [Authorize(Roles = "Admin,Doctor")]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = _appointmentRepository.GetByIdWithModels(id.Value);

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
                //Animals = _animalRepository.GetComboAnimals(user.UserName),
                ClientUsername = user.UserName,
                //Date = DateTime.Today, //Para começar com a data currente
                //Appointments = _appointmentRepository.GetAllWithModels()
            };

            //ViewBag.appointments = GetScheduleData();
            return View(model);
        }

        public async Task<JsonResult> GetData(string username)  // Here we get the Start and End Date and based on that can filter the data and return to Scheduler --VER Passing additional parameters to the server
        {

            if (!this.User.IsInRole(RoleNames.Administrator)) //TESTE SE ESTE PEDAÇO DE CODIGO NAO TRAZ PROBLEMAS. ADMIN TRAZ TUDO, MEDICO TRAZ AS DELE, CLIENT NAO TRAZ NADA
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

            foreach (var item in data) //TODO NAO DEIXAR ABRIR O EDITOR PARA CONSULTAS MARCADAS
            {
                if (item.Client.User.UserName != username)
                {
                    item.IsReadonly = true;
                    //item.Subject = "Date not available";
                }
                else
                {
                    //item.Subject = "Your appointment";
                }
            }

            return Json(data);
        }
        
        public async Task<JsonResult> GetLists(AppointmentViewModel Consulta)  // Here we get the Start and End Date and based on that can filter the data and return to Scheduler --VER Passing additional parameters to the server
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

        public JsonResult GeSpecializationstList(AppointmentViewModel Consulta)  // Here we get the Start and End Date and based on that can filter the data and return to Scheduler --VER Passing additional parameters to the server
        {
            //TODO MUDAR ISTO PARA ID
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

            if (param.action == "insert" || (param.action == "batch" && param.added != null)) // this block of code will execute while inserting the appointments
            {
                var value = (param.action == "insert") ? param.value : param.added[0];

                var client = await _clientRepository.GetClientWithUserAsync(value.ClientUsername);

                var doctor = await _doctorRepository.GetDoctorWithUserAsync(value.DoctorId);

                var animal = await _animalRepository.GetAnimalWithUserAsync(value.AnimalId);

                var specialization = await _specializationRepository.GetByIdAsync(value.SpecializationId);

                Appointment appointment = new Appointment()
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

                var data = _appointmentRepository.GetAllWithModels().ToList();

                foreach (var item in data)
                {
                    //item.IsReadonly = true;
                    //item.Subject = "Not available";
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

                var client = await _clientRepository.GetClientByUserAsync(user);

                var animal = await _animalRepository.GetAnimalWithUserAsync(model.AnimalId);

                var doctor = await _doctorRepository.GetDoctorWithUserAsync(model.DoctorId);
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

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}
