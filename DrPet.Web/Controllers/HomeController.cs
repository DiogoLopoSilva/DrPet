using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DrPet.Web.Models;
using DrPet.Web.Data.Repositories;

namespace DrPet.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public HomeController(IClientRepository clientRepository, IAppointmentRepository appointmentRepository)
        {
            _clientRepository = clientRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel
            {
                AppointmentsTotal = await _appointmentRepository.GetAppointmentsTotal(),
                ClientsTotal = await _clientRepository.GetClientsTotal()
            };

            return View(model);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Services()
        { 
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("error/404")]
        public IActionResult Error404()
        {
            return View();
        }
    }
}
