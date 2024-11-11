using Appointments.API.Interfaces;

namespace Appointments.API.Services
{
    public class ExpirationBackgroundService : BackgroundService
    {
        private readonly IAppointmentsRepository _appointments;
        private readonly ILogger<ExpirationBackgroundService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

        public ExpirationBackgroundService(IAppointmentsRepository repository, ILogger<ExpirationBackgroundService> logger)
        {
            _appointments = repository;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Starting expiration check.");
                try
                {
                    await ExpireAppointments();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during expiration check.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task ExpireAppointments()
        {
            var appointments = await _appointments.GetAllAppointments();
            var currentTime = DateTime.Now;

            foreach (var appointment in appointments)
            {
                if (appointment.Date < DateOnly.FromDateTime(currentTime) ||
                    (appointment.Date == DateOnly.FromDateTime(currentTime) &&
                     appointment.Time < TimeOnly.FromDateTime(currentTime)) &&
                     appointment.Status == "Created")
                {
                    appointment.Status = "Expired";
                    await _appointments.UpdateAppointment(appointment);
                    _logger.LogInformation($"Appointment with Id {appointment.Id} expired.");
                }
            }
        }
    }
}
