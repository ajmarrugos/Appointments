using Appointments.API;
using Appointments.API.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<IRepository, LocalRepository>();
builder.Services.AddOutputCache(opt => 
    opt.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(30));

var app = builder.Build();

app.UseOutputCache();

app.MapControllers();

app.Run();