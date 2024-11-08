using Appointments.API.Models;

namespace Appointments.API.Interfaces
{
    public interface IAppointmentsRepository
    {
        IEnumerable<Appointment> GetAllAppointments();
        Appointment? GetAppointmentById(Guid id);
        IEnumerable<Appointment> GetAppointmentsBySender(string senderEmail);
        Appointment CreateAppointment(Appointment appointment);
        Appointment? RescheduleAppointment(Guid id, DateTime newDate);
        Appointment? SignoffAppointment(Guid id, AppointmentStatus status);
        bool DeleteAppointment(Guid id);
    }
}
