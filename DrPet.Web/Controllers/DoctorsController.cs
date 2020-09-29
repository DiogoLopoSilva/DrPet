using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DrPet.Web.Data;
using DrPet.Web.Data.Entities;
using DrPet.Web.Helpers;
using DrPet.Web.Data.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace DrPet.Web.Controllers
{
    [Authorize(Roles = RoleNames.Administrator)]
    public class DoctorsController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IDoctorRepository _doctorRepository;

        public DoctorsController(DataContext context,
            IUserHelper userHelper,
            IDoctorRepository doctorRepository)
        {
            _context = context;
            _userHelper = userHelper;
            _doctorRepository = doctorRepository;
        }

        // GET: Doctors
        public IActionResult Index()
        {
            return View(_doctorRepository.GetDoctors());
        }

        // POST: Doctors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Specialization,Id")] Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(doctor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(doctor);
        }

        public async Task<IActionResult> DeleteDoctor(string username)
        {
            if (username == null)
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByEmailAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            var doctor = await _doctorRepository.GetDoctorByUserAsync(user);
            if (doctor == null)
            {
                return NotFound();
            }

            await _doctorRepository.DeleteDoctorWithUserAsync(doctor);

            return this.RedirectToAction(nameof(Index));
        }           
    }
}
