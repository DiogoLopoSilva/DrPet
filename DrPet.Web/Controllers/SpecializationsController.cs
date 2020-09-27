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

namespace DrPet.Web.Controllers
{
    public class SpecializationsController : Controller
    {
        private readonly DataContext _context;
        private readonly ISpecializationRepository _specializationRepository;

        public SpecializationsController(DataContext context, ISpecializationRepository specializationRepository)
        {
            _context = context;
            _specializationRepository = specializationRepository;
        }

        // GET: Specializations
        public ActionResult Index()
        {
            return View(_specializationRepository.GetSpecializations());
        }

        // GET: Specializations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialization = await _context.Specializations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specialization == null)
            {
                return NotFound();
            }

            return View(specialization);
        }

        // GET: Specializations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Specializations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Specialization specialization)
        {
            if (ModelState.IsValid)
            {
                await _specializationRepository.CreateUniqueAsync(specialization);
                return RedirectToAction(nameof(Index));
            }
            return View(specialization);
        }

        // GET: Specializations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialization = await _specializationRepository.GetByIdAsync(id.Value);
            if (specialization == null)
            {
                return NotFound();
            }
            return View(specialization);
        }

        // POST: Specializations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Specialization specialization)
        {
            if (ModelState.IsValid)
            {
                await _specializationRepository.UpdateAsync(specialization);

                return RedirectToAction(nameof(Index));
            }
            return View(specialization);
        }

        public async Task<IActionResult> DeleteSpecialization(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (id.Value==1) //TODO POR MENSAGEM A DIZER QUE NAO PODE APAGAR O GENERAL
            {
                return this.RedirectToAction("Index");
            }

            await _specializationRepository.DeleteByIdAsync(id.Value);
            return this.RedirectToAction("Index");
        }
    }
}
