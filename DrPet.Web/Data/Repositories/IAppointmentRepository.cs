using DrPet.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Repositories
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        IQueryable<Appointment> GetAllWithModels();
        IEnumerable<Appointment> GetAnimalAppointments(int id);
        Task<IQueryable<Appointment>> GetAppointmentsAsync(string userName);

        Task<IQueryable<Appointment>> GetAppointmentsByStatusAsync(string userName, string status);
        Task<int> GetAppointmentsTotal();
        Task<Appointment> GetByIdWithModelsAsync(int id);

        Task<IQueryable<Appointment>> GetDoctorsAppointmentsAsync(string userName);

    }
}
