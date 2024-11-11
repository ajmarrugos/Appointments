using Appointments.API.Interfaces;
using Appointments.API.Models;

namespace Appointments.API.Services
{
    public class ManagerService : IManagerService
    {
        private readonly IAppointmentsRepository _appointments;

        public ManagerService(IAppointmentsRepository appointments)
        {
            _appointments = appointments;
        }

        public async Task<bool> ManagerExists(string email)
        {
            // Retrieve all users and check if a manager with the given email exists
            var users = await _appointments.GetManagerUsers();
            return users.Any(u => u.Email == email && u.Role == "Manager");
        }

        public async Task CreateManager(string email)
        {
            var manager = new User
            {
                Email = email,
                Role = "Manager"
            };

            await _appointments.SubscribeManager(manager);
        }
    }
}
