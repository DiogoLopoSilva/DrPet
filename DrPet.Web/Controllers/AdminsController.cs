using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DrPet.Web.Data;
using DrPet.Web.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using DrPet.Web.Data.Repositories;
using DrPet.Web.Helpers;

namespace DrPet.Web.Controllers
{
    [Authorize(Roles = RoleNames.Administrator)]
    public class AdminsController : Controller
    {
        private readonly DataContext _context;
        private readonly IAdminRepository _adminRepository;
        private readonly IUserHelper _userHelper;

        public AdminsController(DataContext context, IAdminRepository adminRepository,
            IUserHelper userHelper)
        {
            _context = context;
            _adminRepository = adminRepository;
            _userHelper = userHelper;
        }

        // GET: Admins
        public IActionResult Index()
        {
            return View(_adminRepository.GetAdmins());
        }      

        public async Task<IActionResult> DeleteAdmin(string username)
        {
            if (username == null)
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByEmailAsync(username);

            if (user == null || user.UserName == this.User.Identity.Name)
            {
                return NotFound();
            }

            var admin = await _adminRepository.GetAdminByUserAsync(user);
            if (admin == null)
            {
                return NotFound();
            }

            await _adminRepository.DeleteAdminWithUser(admin);

            return this.RedirectToAction(nameof(Index));
        }     
    }
}
