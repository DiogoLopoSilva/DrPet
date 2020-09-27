using DrPet.Web.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Repositories
{
    public class SpecializationRepository : GenericRepository<Specialization>, ISpecializationRepository
    {
        private readonly DataContext _context;

        public SpecializationRepository(DataContext context)
            :base(context)
        {
            _context = context;
        }

        public IQueryable GetSpecializations()
        {
            return _context.Specializations.Where(s => !s.IsDeleted);
        }

        public async Task CreateUniqueAsync(Specialization addSpecialization) //Se ja existe 
        {
            var specialization = await _context.Specializations.FirstOrDefaultAsync(s => s.Name.ToLower() == addSpecialization.Name.ToLower() && s.IsDeleted);

            if (specialization != null)
            {
                specialization.IsDeleted = false;

                _context.Specializations.Update(specialization);
            }
            else
            {
                _context.Specializations.Add(addSpecialization);
            }

            await _context.SaveChangesAsync();
        }

        public Specialization GetById(int Id)
        {
            return _context.Specializations.FirstOrDefault(s => s.Id == Id);
        }

        public IEnumerable<SelectListItem> GetComboSpecializations()
        {
            var list = _context.Specializations.Where(s => !s.IsDeleted).Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()

            }).OrderBy(l => l.Text).ToList();

            var others = list.FirstOrDefault(s => s.Text == "Others");

            if (others!=null)
            {
                list.Remove(others);
                list.Add(others);
            };

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a Specialization...)",
                Value = "0"
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboSpecializationsAppointments()
        {
            var list = _context.Specializations.Where(s => !s.IsDeleted).Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()

            }).OrderBy(l => l.Text).ToList();

            var others = list.FirstOrDefault(s => s.Text == "Others");

            if (others != null)
            {
                list.Remove(others);
                list.Add(others);
            };

            return list;
        }

        public async Task DeleteByIdAsync(int id)
        {
            var specialization = await _context.Specializations.FindAsync(id);
            if (specialization == null)
            {
                return;
            }

            _context.Specializations.Remove(specialization);
            await _context.SaveChangesAsync();
        }
    }
}
