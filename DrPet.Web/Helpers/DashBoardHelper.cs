using DrPet.Web.Data;
using DrPet.Web.Data.Entities;
using DrPet.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Helpers
{
    public class DashBoardHelper : IDashBoardHelper
    {
        private readonly DataContext _context;

        public DashBoardHelper(DataContext context)
        {
            _context = context;
        }

        public DashBoardViewModel GetDashBoardInfo()
        {
            var total = _context.Appointments.Where(a => a.StartTime.Date == DateTime.Today && !a.IsDeleted);

            int inProgressCount = total.Where(a => a.Status == "InProgress").Count();
            int CompletedCount = total.Where(a => a.Status == "Completed").Count();
            int WaitingCount = total.Where(a => a.Status == "Waiting").Count();
            int ConfirmedCount = total.Where(a => a.Status == "Confirmed").Count();

            List<DoughnutData> donut = new List<DoughnutData>
            {
                new DoughnutData { xValue =  "In Progress", yValue = inProgressCount,text=inProgressCount.ToString() },
                new DoughnutData { xValue =  "Completed", yValue = CompletedCount,text=CompletedCount.ToString()},
                new DoughnutData { xValue =  "Waiting Aproval", yValue = WaitingCount,text=WaitingCount.ToString()},
                new DoughnutData { xValue =  "Confirmed", yValue = ConfirmedCount,text=ConfirmedCount.ToString()},
            };

            List<DoughnutData> donutSorted = new List<DoughnutData>();

            foreach (var item in donut)
            {
                if (item.yValue!=0)
                {
                    donutSorted.Add(item);
                }
            }

            return new DashBoardViewModel
            {
                Admins = _context.Admins.Where(a => !a.IsDeleted).Count(),
                Clients = _context.Clients.Where(a => !a.IsDeleted).Count(),
                Doctors = _context.Doctors.Where(a => !a.IsDeleted).Count(),
                Animals = _context.Animals.Where(a => !a.IsDeleted).Count(),
                Appointments = _context.Appointments.Where(a => !a.IsDeleted && a.Status == "Completed").Count(),
                ChartData = donutSorted,
                LatestClients = _context.Clients.Include(c => c.User).Where(c => !c.IsDeleted && c.User.EmailConfirmed).OrderByDescending(c => c.User.DateCreated).Take(10).ToList()
            };
        }
    }
}
