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
using DrPet.Web.Helpers;

namespace DrPet.Web.Controllers
{
    [Authorize(Roles =RoleNames.Administrator)]
    public class ClientsController : Controller
    {
        private readonly DataContext _context;
        private readonly IClientRepository _clientRepository;
        private readonly IUserHelper _userHelper;

        public ClientsController(DataContext context, IClientRepository clientRepository, IUserHelper userHelper)
        {
            _context = context;
            _clientRepository = clientRepository;
            _userHelper = userHelper;
        }

        // GET: Clients
        public IActionResult Index()
        {
            //return View(await _context.Clients.ToListAsync());

            return View(_clientRepository.GetClients());
        }

        public async Task<IActionResult> DeleteClient(string username)
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

            var client = await _clientRepository.GetClientByUserAsync(user);
            if (client == null)
            {
                return NotFound();
            }

            await _clientRepository.DeleteClientWithUser(client);

            return this.RedirectToAction(nameof(Index));
        }
    }
}
