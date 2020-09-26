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

namespace DrPet.Web.Controllers
{
    [Authorize]
    public class AnimalsController : Controller
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IImageHelper _imageHelper;

        public AnimalsController(IAnimalRepository animalRepository,
            IUserHelper userHelper,
            IConverterHelper converterHelper,
            IImageHelper imageHelper)
        {
            _animalRepository = animalRepository;
            _userHelper = userHelper;
            _converterHelper = converterHelper;
            _imageHelper = imageHelper;
        }

        //GET: Animals
        public async Task<IActionResult> Index()
        {
            return View(await _animalRepository.GetAnimalsAsync(this.User.Identity.Name));
        }

        //// GET: Animals
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _animalRepository.GetAnimalsAsync(this.User.Identity.Name));
        //}

        // GET: Animals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("AnimalNotFound");
            }

            var animal = await _animalRepository.GetByIdAsync(id.Value);
            if (animal == null)
            {
                return new NotFoundViewResult("AnimalNotFound");
            }

            return View(animal);
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
                if ((this.User.Identity.Name != model.User.UserName) && !this.User.IsInRole("Admin")) //TODO VER SE NAO DEVO POR SO O USERNAME
                {
                    return NotFound();
                }

                var user = await _userHelper.GetUserByEmailAsync(model.User.UserName);

                if (user==null)
                {
                    return NotFound();
                }

                var path = string.Empty;

                if (model.ImageFile != null) //Verificar se preciso de ver se a Length é maior que 0
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile,"Animals");
                }

                var animal = _converterHelper.ToAnimal(model, path, true);

                animal.User = user;
                await _animalRepository.CreateAsync(animal);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Animals/Edit/5
        public async Task<IActionResult> Edit(int? id) //TODO MUDAR PARA O CLIENTE SO CONSEGUIR ALTERAR ANIMAIS QUE SAO SEUS
        {
            if (id == null)
            {
                return new NotFoundViewResult("AnimalNotFound");
            }

            var animal = await _animalRepository.GetByIdAsync(id.Value);
            if (animal == null)
            {
                return new NotFoundViewResult("AnimalNotFound");
            }
            var model = _converterHelper.ToAnimalViewModel(animal);

            return View(model);
        }

        // POST: Animals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AnimalViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var path = model.ImageUrl;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        path = await _imageHelper.UploadImageAsync(model.ImageFile,"Animals");
                    }

                    var animal = _converterHelper.ToAnimal(model, path, false);

                    animal.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                    await _animalRepository.UpdateAsync(animal);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _animalRepository.ExistsAsync(model.Id))
                    {
                        return new NotFoundViewResult("AnimalNotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Animals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("AnimalNotFound");
            }

            var animal = await _animalRepository.GetByIdAsync(id.Value);
            if (animal == null)
            {
                return new NotFoundViewResult("AnimalNotFound");
            }

            return View(animal);
        }

        // POST: Animals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var animal = await _animalRepository.GetByIdAsync(id);
            await _animalRepository.DeleteAsync(animal);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult AnimalNotFound()
        {
            return View();
        }

        public async Task<IActionResult> DeleteAnimal(int? id)
        {    
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _animalRepository.GetByIdAsync(id.Value);

            if (animal==null)
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
    }
}
