using Appointments.API.Models;

namespace Appointments.API
{
    public interface IRepository
    {
        List<Appointment> GetAppointments();
        Task<Appointment?> GetAppointmentsById(Guid id);
        Task<Appointment?> GetAppointmentsBySender(string email);
    }
}
