using System.Collections.Concurrent;
using Appointments.API.Models;

namespace Appointments.API.Repository
{
    public class LocalRepository : IRepository
    {

        // Concurrent dictionary to store appointments while development
        private readonly ConcurrentDictionary<Guid, Appointment> _appointments = new();

        // Instance constructor for a local repository with pre-loaded appointments
        public LocalRepository()
        {
            var appointment1 = new Appointment
            {
                Id = Guid.NewGuid(),
                SenderEmail = "ajmarrugos@gmail.com",
                RecipientEmail = "alberto.marrugo@unosquare.com",
                ApptName = "Technical Assesment",
                ApptDate = DateTime.Now.AddDays(2),  // Future date
                Status = AppointmentStatus.Created
            };

            var appointment2 = new Appointment
            {
                Id = Guid.NewGuid(),
                SenderEmail = "alberto.marrugo@unosquare.com",
                RecipientEmail = "ajmarrugos@gmail.com",
                ApptName = "Assesment Feedback",
                ApptDate = DateTime.Now.AddDays(3),  // Future date
                Status = AppointmentStatus.Created
            };

            _appointments[appointment1.Id] = appointment1;
            _appointments[appointment2.Id] = appointment2;
        }

        /// <summary>
        /// Retrieves all appointments currently in the repository.
        /// </summary>
        /// <returns>An enumerated list of appointments</returns>
        public IEnumerable<Appointment> GetAllAppointments()
        {
            return _appointments.Values.AsEnumerable();
        }

        /// <summary>
        /// Retrieves an appointment by its unique identifier.
        /// Returns null if no appointment with the specified ID is found.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>An specific appointment</returns>
        public Appointment? GetAppointmentById(Guid id)
        {
            _appointments.TryGetValue(id, out var appointment);
            return appointment;
        }

        /// <summary>
        /// Retrieves all appointments for a given sender's email.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Appointments by Sender</returns>
        public IEnumerable<Appointment> GetAppointmentsBySender(string senderEmail)
        {
            return _appointments.Values.Where(a => a.SenderEmail == senderEmail);
        }

        /// <summary>
        /// Adds a new appointment to the repository and returns it.
        /// </summary>
        /// <param name="appointment"></param>
        /// <returns>The appointment created</returns>
        public Appointment CreateAppointment(Appointment appointment)
        {
            _appointments[appointment.Id] = appointment;
            return appointment;
        }

        /// <summary>
        /// Updates the date of an existing appointment and sets its status to Rescheduled.
        /// Returns the updated appointment, or null if the appointment ID is not found.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newApptDate"></param>
        /// <returns>The appointment rescheduled</returns>
        public Appointment? RescheduleAppointment(Guid id, DateTime newDate)
        {
            if (_appointments.TryGetValue(id, out var appointment))
            {
                appointment.ApptDate = newDate;
                appointment.Status = AppointmentStatus.Rescheduled;
                return appointment;
            }
            return null;
        }

        /// <summary>
        /// Changes the status of an appointment to Approved or Canceled.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns>The updated appointment, or null if the appointment ID is not found.</returns>
        public Appointment? SignoffAppointment(Guid id, AppointmentStatus status)
        {
            if (_appointments.TryGetValue(id, out var appointment) &&
                (status == AppointmentStatus.Approved || status == AppointmentStatus.Canceled))
            {
                appointment.Status = status;
                return appointment;
            }
            return null;
        }

        /// <summary>
        /// Deletes an appointment only if its status is Canceled.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if the appointment was deleted, False if it wasn't found or was not Canceled.</returns>
        public bool DeleteAppointment(Guid id)
        {
            if (_appointments.TryGetValue(id, out var appointment) &&
                appointment.Status == AppointmentStatus.Canceled)
            {
                return _appointments.TryRemove(id, out _);
            }
            return false;
        }
    }
}
