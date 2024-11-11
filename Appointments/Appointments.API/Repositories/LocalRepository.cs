using Appointments.API.Interfaces;
using Appointments.API.Models;

namespace Appointments.API.Repositories
{
    // Repository for handling appointment operations in a local memory
    public class LocalRepository : IAppointmentsRepository
    {
        // List where to store appointments while development
        private readonly List<Appointment> _appointments;

        // Instance constructor for a local repository with pre-loaded appointments
        public LocalRepository()
        {
            // Pre-load some sample data
            _appointments = new List<Appointment>
            {
                new Appointment
                {
                    SenderEmail = "sender1@example.com",
                    RecipientEmail = "recipient1@example.com",
                    ApptName = "Test Appointment 1",
                    ApptDate = DateTime.Now.AddDays(1),
                    Status = AppointmentStatus.Created
                },
                new Appointment
                {
                    SenderEmail = "sender2@example.com",
                    RecipientEmail = "recipient2@example.com",
                    ApptName = "Test Appointment 2",
                    ApptDate = DateTime.Now.AddDays(2),
                    Status = AppointmentStatus.Created
                }
            };
        }

        /// <summary>
        /// Retrieves all appointments currently in the repository.
        /// </summary>
        /// <returns>An enumerated list of appointments</returns>
        public async Task<IEnumerable<Appointment>> GetAllAppointments()
        {
            return await Task.FromResult(_appointments);
        }

        /// <summary>
        /// Retrieves an appointment by its unique identifier.
        /// Returns null if no appointment with the specified ID is found.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>An specific appointment</returns>
        public async Task<Appointment> GetAppointmentById(Guid id)
        {
            var appointment = _appointments.FirstOrDefault(a => a.Id == id);
            return await Task.FromResult(appointment);
        }

        /// <summary>
        /// Retrieves all appointments for a given sender's email.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Appointments by Sender</returns>
        public async Task<IEnumerable<Appointment>> GetAppointmentsBySender(string senderEmail)
        {
            var appointments = _appointments.Where(a => a.SenderEmail.Equals(senderEmail, StringComparison.OrdinalIgnoreCase)).ToList();
            return await Task.FromResult(appointments);
        }

        /// <summary>
        /// Adds a new appointment to the repository and returns it.
        /// </summary>
        /// <param name="appointment"></param>
        /// <returns>The appointment created</returns>
        public async Task<Appointment> CreateAppointment(Appointment appointment)
        {
            appointment.Id = Guid.NewGuid(); // Ensure a new ID for each appointment
            _appointments.Add(appointment);
            return await Task.FromResult(appointment);
        }

        /// <summary>
        /// Updates the date of an existing appointment and sets its status to Rescheduled.
        /// Returns the updated appointment, or null if the appointment ID is not found.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newApptDate"></param>
        /// <returns>The appointment rescheduled</returns>
        public async Task<Appointment> RescheduleAppointment(Guid id, DateTime newDate)
        {
            var appointment = _appointments.FirstOrDefault(a => a.Id == id);
            if (appointment != null)
            {
                appointment.ApptDate = newDate;
                appointment.Status = AppointmentStatus.Rescheduled;
            }
            return await Task.FromResult(appointment);
        }

        /// <summary>
        /// Changes the status of an appointment to Approved or Canceled.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns>The updated appointment, or null if the appointment ID is not found.</returns>
        public async Task<Appointment> SignoffAppointment(Guid id, AppointmentStatus status)
        {
            var appointment = _appointments.FirstOrDefault(a => a.Id == id);
            if (appointment != null)
            {
                appointment.Status = status;
            }
            return await Task.FromResult(appointment);
        }

        /// <summary>
        /// Deletes an appointment only if its status is Canceled.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if the appointment was deleted, False if it wasn't found or was not Canceled.</returns>
        public async Task<bool> DeleteAppointment(Guid id)
        {
            var appointment = _appointments.FirstOrDefault(a => a.Id == id);
            if (appointment != null && appointment.Status == AppointmentStatus.Canceled)
            {
                _appointments.Remove(appointment);
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }
    }
}
