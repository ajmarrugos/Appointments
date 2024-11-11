using Appointments.API.Models;
using Appointments.API.Interfaces;

namespace Appointments.API.Interfaces
{
    public interface IManagerService
    {
        bool ManagerExists(string email);
        void CreateManager(string email, string password);
    }

    public class ManagerService : IManagerService
    {
        private readonly IAppointmentsRepository _appointments;

        public ManagerService(IAppointmentsRepository appointments)
        {
            _appointments = appointments;
        }

        public bool ManagerExists(string email)
        {
            // Check if the manager exists in the repository
            return _appointments.GetAllUsers().Any(u => u.Email == email && u.Role == "Manager");
        }

        public void CreateManager(string email, string password)
        {
            // If the manager does not exist, create it
            var manager = new User
            {
                Email = email,
                Password = password,
                Role = "Manager"
            };

            _appointments.AddUser(manager);  // Assuming an AddUser method exists in your repository
        }
    }
}
