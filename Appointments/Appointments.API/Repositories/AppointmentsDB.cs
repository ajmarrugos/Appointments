using Appointments.API.Data;
using Appointments.API.Models;
using Appointments.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Appointments.API.Repositories
{
    // SQL database repository for appointments, implementing IAppointmentsRepository
    public class AppointmentsDB : IAppointmentsRepository
    {
        // DB context where to store Data
        private readonly AppDbContext _context;
        private readonly ILogger<AppointmentsDB> _logger;


        // Instance constructor for loading context from a DataBase
        public AppointmentsDB(AppDbContext context, ILogger<AppointmentsDB> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all appointments from the database
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Appointment>> GetAllAppointments() =>
            await _context.Appointments.ToListAsync();

        /// <summary>
        /// Retrieves a specific appointment by its unique identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Null if no appointment with the specified ID is found.</returns>
        public async Task<Appointment?> GetAppointmentById(int id) =>
            await _context.Appointments.FindAsync(id);

        /// <summary>
        /// Retrieves all appointments associated with the given attribute in the request
        /// </summary>
        /// <param name="senderEmail"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Appointment>> QueryAppointments(string attribute, string value)
        {
            var query = _context.Appointments.AsQueryable();

            query = attribute.ToLower() switch
            {
                "id" => query.Where(a => a.Id.ToString() == value),
                "name" => query.Where(a => a.Name == value),
                "state" => query.Where(a => a.Status == value),
                "date" => query.Where(a => a.Date.ToString("dd-MM-yyyy") == value),
                "requestor" => query.Where(a => a.Sender == value),
                _ => query
            };

            return await query.ToListAsync();
        }

        /// <summary>
        /// Adds a new appointment to the database and saves changes
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
        /// Updates the date or status an existing appointment
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newDate"></param>
        /// <returns>The updated appointment, or null if the appointment ID is not found.</returns>
        public async Task UpdateAppointment(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes an appointment only if its status is Canceled
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if the appointment was deleted, False if it wasn't found or was not Canceled.</returns>
        public async Task DeleteAppointment(int id)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(id);
                if (appointment != null)
                {
                    _context.Appointments.Remove(appointment);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Error deleting appointment with Id {id}.");
                throw new Exception("Could not delete appointment. Please try again.");
            }
        }

        /// <summary>
        /// Method to get all users
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<User>> GetAllUsersAsync() =>
            await _context.Users.ToListAsync();

        /// <summary>
        /// Add users to the repository
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public Task<IEnumerable<User>> GetManagerUsers()
        {
            throw new NotImplementedException();
        }

        public Task SubscribeManager(User user)
        {
            throw new NotImplementedException();
        }
    }
}
