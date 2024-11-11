using Microsoft.EntityFrameworkCore;
using Appointments.API.Repositories;
using Appointments.API.Interfaces;
using Appointments.API.Data;
using Appointments.API.Services;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

ConfigureServices(builder.Services, builder.Configuration);

app.UseOutputCache();

app.MapControllers();

app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Read Manager credentials from environment variables (set in launchSettings.json)
    var managerEmail = builder.Configuration["ManagerEmail"];
    var managerPassword = builder.Configuration["ManagerPassword"];

    // Initialize the manager role at startup
    InitializeManagerRole(app, managerEmail, managerPassword);

    // Register Controllers
    services.AddControllers();

    // Registers OutputCache for speeding endpoint response time
    builder.Services.AddOutputCache(opt =>
        opt.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(60));

    builder.Services.AddScoped<IManagerService, ManagerService>();
    // services.AddSingleton<IAppointmentsRepository, AppointmentsDB>(); // Using the SQL Database repository
    services.AddSingleton<IAppointmentsRepository, LocalRepository>(); // Using the Local Repository

    // Register the DbContext with connection setting
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("AppointmentsDb")));

    // Register the Expiration Background Service
    builder.Services.AddHostedService<ExpirationBackgroundService>();
    builder.Services.AddLogging(logging =>
    {
        logging.AddConsole();
        logging.AddDebug();
    });
}

static void InitializeManagerRole(WebApplication app, string managerEmail, string managerPassword)
{
    // Your initialization logic here
    var managerService = app.Services.GetRequiredService<IManagerService>();

    if (!managerService.ManagerExists(managerEmail))
    {
        managerService.CreateManager(managerEmail, managerPassword);
    }
}