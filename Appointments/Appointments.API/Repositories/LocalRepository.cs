using Appointments.API.Interfaces;
using Appointments.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Appointments.API.Repositories
{
    // Repository for handling appointment operations in a local memory
    public class LocalRepository : IAppointmentsRepository
    {
        // List where to store appointments & users while development
        private readonly List<Appointment> _appointments = new();
        private readonly List<User> _users = new();
        private int _nextId = 0; // Initial value for the auto-increment Id

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
        public Task<Appointment> GetAppointmentById(int id)
        {
            var appointment = _appointments.FirstOrDefault(a => a.Id == id);
            return Task.FromResult(appointment);
        }

        /// <summary>
        /// Retrieves all appointments for a given sender's email.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Appointments by Sender</returns>
        public async Task<IEnumerable<Appointment>> QueryAppointments(string attribute, string value)
        {
            var query = _appointments.AsQueryable();

            query = attribute.ToLower() switch
            {
                "id" => query.Where(a => a.Id.ToString() == value),
                "name" => query.Where(a => a.Name == value),
                "state" => query.Where(a => a.Status == value),
                "date" => query.Where(a => a.Date.ToString("dd-MM-yy") == value),
                "requestor" => query.Where(a => a.Sender == value),
                _ => query
            };

            return await query.ToListAsync();
        }

        /// <summary>
        /// Adds a new appointment to the repository and returns it.
        /// </summary>
        /// <param name="appointment"></param>
        /// <returns>The appointment created</returns>
        public Task<Appointment> CreateAppointment(Appointment appointment)
        {
            appointment.Id = _nextId++;
            appointment.Status = "created";
            _appointments.Add(appointment);
            return Task.FromResult(appointment);
        }

        /// <summary>
        /// Updates the date of an existing appointment and sets its status to Rescheduled.
        /// Returns the updated appointment, or null if the appointment ID is not found.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newApptDate"></param>
        /// <returns>The appointment rescheduled</returns>
        public Task UpdateAppointment(Appointment appointment)
        {
            var existing = _appointments.FirstOrDefault(a => a.Id == appointment.Id);
            if (existing != null)
            {
                _appointments.Remove(existing);
                _appointments.Add(appointment);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Deletes an appointment only if its status is Canceled.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if the appointment was deleted, False if it wasn't found or was not Canceled.</returns>
        public Task DeleteAppointment(int id)
        {
            var appointment = _appointments.FirstOrDefault(a => a.Id == id);
            if (appointment != null)
            {
                _appointments.Remove(appointment);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Adding user method
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task SubscribeManager(User user)
        {
            _users.Add(user);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<User>> GetManagerUsers() =>
            Task.FromResult<IEnumerable<User>>(_users);

        /// <summary>
        /// Getting User method
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<User> GetUserById(string email)
        {
            throw new NotImplementedException();
        }
    }
}
