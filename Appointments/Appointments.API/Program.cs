using Microsoft.EntityFrameworkCore;
using Appointments.API.Repositories;
using Appointments.API.Interfaces;
using Appointments.API.Data;
using Appointments.API.Services;

namespace Appointments.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure services in a separate method
        ConfigureServices(builder, args);

        var app = builder.Build();

        // Read Manager credentials from environment variables (set in launchSettings.json)
        var managerEmail = builder.Configuration["ManagerEmail"];
        var managerPassword = builder.Configuration["ManagerPassword"];

        // Initialize the manager role at startup
        await InitializeManagerRole(app, managerEmail);

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        await app.RunAsync();
    }

    private static void ConfigureServices(WebApplicationBuilder builder, string[] args)
    {
        // Determine repository implementation based on startup argument
        if (args.Length > 0 && args[0].ToLower() == "prod")
        {
            // Use SQL Server repository for production
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("AppointmentsDb")));
            builder.Services.AddSingleton<IAppointmentsRepository, AppointmentsDB>();
        }
        else
        {
            // Use in-memory repository for development or testing
            builder.Services.AddScoped<IAppointmentsRepository, LocalRepository>();
        }

        // Register application services
        builder.Services.AddScoped<IManagerService, ManagerService>();

        // Add controllers and Swagger for API documentation
        builder.Services.AddControllers();
    }

    private static async Task InitializeManagerRole(WebApplication app, string managerEmail)
    {
        using var scope = app.Services.CreateScope();
        var managerService = scope.ServiceProvider.GetRequiredService<IManagerService>();

        // Check if the manager exists, and create if necessary
        bool managerExists = await managerService.ManagerExists(managerEmail);
        if (!managerExists)
        {
            await managerService.CreateManager(managerEmail);
        }
    }
}