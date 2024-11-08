using Appointments.API.Models;

namespace Appointments.API.Interfaces
{
    public interface IAppointmentsRepository
    {
        Task<IEnumerable<Appointment>> GetAllAppointments();
        Task<Appointment> GetAppointmentById(Guid id);
        Task<IEnumerable<Appointment>> GetAppointmentsBySender(string senderEmail);
        Task<Appointment> CreateAppointment(Appointment appointment);
        Task<Appointment> RescheduleAppointment(Guid id, DateTime newDate);
        Task<Appointment> SignoffAppointment(Guid id, AppointmentStatus status);
        Task<bool> DeleteAppointment(Guid id);
    }
}
