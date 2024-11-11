using Appointments.API.Models;

namespace Appointments.API.Interfaces
{
    public interface IAppointmentsRepository
    {
        Task<IEnumerable<Appointment>> GetAllAppointments();
        Task<Appointment> GetAppointmentById(int id);
        Task<IEnumerable<Appointment>> QueryAppointments(string attribute, string value);
        Task<Appointment> CreateAppointment(Appointment appointment);
        Task UpdateAppointment(Appointment appointment);
        Task DeleteAppointment(int id);
    }
}
