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
using DrPet.Web.Models;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using DrPet.Web.Data.Repositories;
using Microsoft.AspNetCore.Http;

namespace DrPet.Web.Controllers
{
    [Authorize]
    public class AnimalsController : Controller
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IAppointmentRepository _appointmentRepository;

        public AnimalsController(IAnimalRepository animalRepository,
            IUserHelper userHelper,
            IConverterHelper converterHelper,
            IImageHelper imageHelper,
            IAppointmentRepository appointmentRepository)
        {
            _animalRepository = animalRepository;
            _userHelper = userHelper;
            _converterHelper = converterHelper;
            _imageHelper = imageHelper;
            _appointmentRepository = appointmentRepository;
        }

        //GET: Animals
        public async Task<IActionResult> Index(string username)
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            if (username != null && await _userHelper.IsUserInRoleAsync(user, RoleNames.Administrator))
            {
                user = await _userHelper.GetUserByEmailAsync(username);
            }

            return View(await _animalRepository.GetAnimalsAsync(user.UserName));
        }       

        // GET: Animals/Create
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

            var model = new AnimalViewModel
            {
                User = user
            };

            return View(model);
        }

        // POST: Animals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AnimalViewModel model)
        {
            if (ModelState.IsValid)
            {
                if ((this.User.Identity.Name != model.User.UserName) && !this.User.IsInRole("Admin"))
                {
                    return NotFound();
                }

                var user = await _userHelper.GetUserByEmailAsync(model.User.UserName);

                if (user==null)
                {
                    return NotFound();
                }

                var animal = _converterHelper.ToAnimal(model, true);

                animal.User = user;
                await _animalRepository.CreateAsync(animal);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Animals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _animalRepository.GetAnimalWithUserAsync(id.Value);

            if (animal == null)
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            if (this.User.IsInRole("Client") && animal.User != user)
            {
                return NotFound();
            }

            var appointments = _appointmentRepository.GetAnimalAppointments(animal.Id);

            var model = _converterHelper.ToAnimalDetailsViewModel(animal, appointments);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AnimalDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var animal = await _animalRepository.GetAnimalWithUserAsync(model.Id);

                if (animal==null)
                {
                    return NotFound();
                }

                if ((this.User.Identity.Name != animal.User.UserName) && !this.User.IsInRole("Admin"))
                {
                    return NotFound();
                }

                model.ImageUrl = animal.ImageUrl;

                animal = _converterHelper.AnimalDetailsToAnimal(animal,model);

                await _animalRepository.UpdateAsync(animal);
            }

            model.User = await _userHelper.GetUserByEmailAsync(model.User.UserName);
            model.Appointments = _appointmentRepository.GetAnimalAppointments(model.Id);
            return View("Details",model);
        }

        public async Task<JsonResult> UploadImage(IFormCollection form)
        {
            string value = Request.Form["id"];

            int id = Convert.ToInt32(value);

            var animal = await _animalRepository.GetAnimalWithUserAsync(id);

            if (this.User.Identity.Name != animal.User.UserName && !this.User.IsInRole("Admin"))
            {
                return Json(new { result = "Failed" });
            }

            if (animal != null)
            {
                IFormFile image = form.Files[0];

                string path = await _imageHelper.UploadImageAsync(image, "Animals");

                animal.ImageUrl = path;

                await _animalRepository.UpdateAsync(animal);

                return Json(new { result = "Success" });
            }

            return Json(new { result = "Failed" });
        }

        public async Task<IActionResult> DeleteAnimal(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _animalRepository.GetByIdAsync(id.Value);

            if (animal == null)
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            if (!this.User.IsInRole("Admin") && animal.User != user)
            {
                return NotFound();
            }

            await _animalRepository.DeleteAnimalAsync(animal);

            return this.RedirectToAction(nameof(Index));
        }
       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var animal = await _animalRepository.GetByIdAsync(id);

            if (animal == null)
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            if (!this.User.IsInRole("Admin") && animal.User != user)
            {
                return NotFound();
            }

            await _animalRepository.DeleteAsync(animal);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult AnimalNotFound()
        {
            return View();
        }          
    }
}
