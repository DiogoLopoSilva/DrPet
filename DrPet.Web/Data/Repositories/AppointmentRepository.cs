﻿using DrPet.Web.Data.Entities;
using DrPet.Web.Helpers;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Repositories
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public AppointmentRepository(DataContext context, IUserHelper userHelper)
            : base(context)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public IQueryable<Appointment> GetAllWithModels()
        {
            return _context.Appointments.Include(a => a.Client)
                  .ThenInclude(c => c.User)
                  .Include(a => a.Animal)
                  .Include(a => a.Specialization)
                  .Include(a => a.Doctor)
                  .ThenInclude(d => d.User)
                  .Where(a => !a.IsDeleted)
                  .OrderBy(a => a.StartTime);
        }

        public async Task<IQueryable<Appointment>> GetAppointmentsAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);

            if (user == null)
            {
                return null;
            }

            if (await _userHelper.IsUserInRoleAsync(user, RoleNames.Administrator))
            {
                return _context.Appointments.Include(a => a.Client)
                      .ThenInclude(c => c.User)
                      .Include(a => a.Animal)
                      .Include(a => a.Specialization)
                      .Include(a => a.Doctor)
                      .ThenInclude(d => d.User)
                      .Where(a => !a.IsDeleted)
                      .OrderBy(a => a.StartTime);
            }

            if (await _userHelper.IsUserInRoleAsync(user, RoleNames.Doctor))
            {
                return _context.Appointments.Include(a => a.Client)
                 .ThenInclude(c => c.User)
                 .Include(a => a.Animal)
                 .Include(a => a.Specialization)
                 .Include(a => a.Doctor)
                 .ThenInclude(d => d.User)
                 .Where(a => a.Doctor.User == user && !a.IsDeleted)
                 .OrderBy(a => a.StartTime);
            }

            return _context.Appointments.Include(a => a.Client)
                  .ThenInclude(c => c.User)
                  .Include(a => a.Animal)
                  .Include(a => a.Specialization)
                  .Include(a => a.Doctor)
                  .ThenInclude(d => d.User)
                  .Where(a => a.Client.User == user && !a.IsDeleted)
                  .OrderBy(a => a.StartTime);
        }

        public async Task<IQueryable<Appointment>> GetAppointmentsByStatusAsync(string userName,string status)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);

            if (user == null)
            {
                return null;
            }

            if (await _userHelper.IsUserInRoleAsync(user, RoleNames.Administrator))
            {
                return _context.Appointments.Include(a => a.Client)
                      .ThenInclude(c => c.User)
                      .Include(a => a.Animal)
                      .Include(a => a.Specialization)
                      .Include(a => a.Doctor)
                      .ThenInclude(d => d.User)
                      .Where(a => a.Status == status && !a.IsDeleted)
                      .OrderBy(a => a.StartTime);
            }

            if (await _userHelper.IsUserInRoleAsync(user, RoleNames.Doctor))
            {
                return _context.Appointments.Include(a => a.Client)
                 .ThenInclude(c => c.User)
                 .Include(a => a.Animal)
                 .Include(a => a.Specialization)
                 .Include(a => a.Doctor)
                 .ThenInclude(d => d.User)
                 .Where(a => a.Doctor.User == user && a.Status == status && !a.IsDeleted)
                 .OrderBy(a => a.StartTime);
            }

            return _context.Appointments.Include(a => a.Client)
                  .ThenInclude(c => c.User)
                  .Include(a => a.Animal)
                  .Include(a => a.Specialization)
                  .Include(a => a.Doctor)
                  .ThenInclude(d => d.User)
                  .Where(a => a.Client.User == user && a.Status == status && !a.IsDeleted)
                  .OrderBy(a => a.StartTime);
        }

        public async Task<IQueryable<Appointment>> GetDoctorsAppointmentsAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);

            if (user == null)
            {
                return null;
            }

            return _context.Appointments.Include(a => a.Client)
                  .ThenInclude(c => c.User)
                  .Include(a => a.Animal)
                  .Include(a => a.Specialization)
                  .Include(a => a.Doctor)
                  .ThenInclude(d => d.User)
                  .Where(a => a.Doctor.User == user)
                  .OrderBy(a => a.StartTime);
        }

        public async Task<Appointment> GetByIdWithModelsAsync(int id)
        {
            return await _context.Appointments.Include(a => a.Client)
                  .ThenInclude(c => c.User)
                  .Include(a => a.Animal)
                  .Include(a => a.Specialization)
                  .Include(a => a.Doctor)
                  .ThenInclude(d => d.User)
                  .FirstOrDefaultAsync(d => d.Id == id);
        }

        public IEnumerable<Appointment> GetAnimalAppointments(int id)
        {
            return _context.Appointments.Include(a => a.Client)
                       .ThenInclude(c => c.User)
                       .Include(a => a.Animal)
                       .Include(a => a.Specialization)
                       .Include(a => a.Doctor)
                       .ThenInclude(d => d.User)
                       .Where(a => a.Animal.Id == id && !a.IsDeleted);
        }
        public async Task<int> GetAppointmentsTotal()
        {
            return await _context.Appointments.Where(a => a.Status == "Completed" && !a.IsDeleted).CountAsync();
        }
    }
}
