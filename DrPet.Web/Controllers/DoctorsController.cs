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
