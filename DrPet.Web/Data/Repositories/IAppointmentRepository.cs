using DrPet.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Repositories
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<bool> ConfirmAppointmentAsync(int id);
        Task CreateAppointmentTemp(AppointmentTemp appointmentTemp);

        Task DeleteAppointmentTempAsync(int id);

        Task<IQueryable<Appointment>> GetAppointmentsAsync(string userName);

        Task<IQueryable<AppointmentTemp>> GetAppointmentsTempAsync(string userName);
    }
}
