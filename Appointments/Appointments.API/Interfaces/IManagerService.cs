namespace Appointments.API.Interfaces
{
    public interface IManagerService
    {
        Task<bool> ManagerExists(string email);
        Task CreateManager(string email);
    }
}
