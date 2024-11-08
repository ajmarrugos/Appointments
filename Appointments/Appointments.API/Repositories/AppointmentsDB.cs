using Appointments.API.Models;
using Appointments.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Appointments.API.Repositories
{
    // SQL database repository for appointments, implementing IAppointmentsRepository.
    public class AppointmentsDB : IAppointmentsRepository
    {
        private readonly AppointmentsDbContext _context;

        public AppointmentsDB(AppointmentsDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all appointments from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Appointment>> GetAllAppointments() =>
            await _context.Appointments.ToListAsync();

        /// <summary>
        /// Retrieves a specific appointment by its unique identifier.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Null if no appointment with the specified ID is found.</returns>
        public async Task<Appointment?> GetAppointmentById(Guid id) =>
            await _context.Appointments.FindAsync(id);

        /// <summary>
        /// Retrieves all appointments associated with a given sender's email.
        /// </summary>
        /// <param name="senderEmail"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Appointment>> GetAppointmentsBySender(string senderEmail) =>
            await _context.Appointments
                .Where(appt => appt.SenderEmail == senderEmail)
                .ToListAsync();

        /// <summary>
        /// Adds a new appointment to the database and saves changes.
        /// </summary>
        /// <param name="appointment"></param>
        /// <returns></returns>
        public async Task<Appointment> CreateAppointment(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        /// <summary>
        /// Updates the date of an existing appointment and sets its status to Rescheduled.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newDate"></param>
        /// <returns>The updated appointment, or null if the appointment ID is not found.</returns>
        public async Task<Appointment?> RescheduleAppointment(Guid id, DateTime newDate)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return null;

            appointment.ApptDate = newDate;
            appointment.Status = AppointmentStatus.Rescheduled;
            await _context.SaveChangesAsync();
            return appointment;
        }

        /// <summary>
        /// Updates the status of an appointment to Approved or Canceled.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns>Th updated appointment, or null if the appointment ID is not found.</returns>
        public async Task<Appointment?> SignoffAppointment(Guid id, AppointmentStatus status)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return null;

            if (status == AppointmentStatus.Approved || status == AppointmentStatus.Canceled)
            {
                appointment.Status = status;
                await _context.SaveChangesAsync();
                return appointment;
            }
            return null;
        }

        /// <summary>
        /// Deletes an appointment only if its status is Canceled.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if the appointment was deleted, False if it wasn't found or was not Canceled.</returns>
        public async Task<bool> DeleteAppointment(Guid id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null && appointment.Status == AppointmentStatus.Canceled)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
