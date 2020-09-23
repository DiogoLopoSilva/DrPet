using DrPet.Web.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public IEnumerable<SelectListItem> GetComboSpecializations()
        {
            var list = _context.Specializations.Select(c => new SelectListItem
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
    }
}
