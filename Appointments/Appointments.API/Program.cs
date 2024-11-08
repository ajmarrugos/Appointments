using Microsoft.EntityFrameworkCore;
using Appointments.API.Repositories;
using Appointments.API.Interfaces;
using Appointments.API.Data;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

app.UseOutputCache();

app.MapControllers();

app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Register Controllers
    services.AddControllers();

    // Registers OutputCache for speeding endpoint response time
    builder.Services.AddOutputCache(opt =>
        opt.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(60));

    // Register repositories as scoped services
    // services.AddSingleton<IAppointmentsRepository, AppointmentsDB>(); // Using the SQL Database repository
    services.AddSingleton<IAppointmentsRepository, LocalRepository>(); // Using the Local Repository

    // Register the DbContext with dependency injection
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("AppointmentsDb")));
}