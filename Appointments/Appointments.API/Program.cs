using System.Text.Json;
using Appointments.API.Models;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

var appointments = new List<Appointment>();

string samples = "appointments.json";

if (File.Exists(samples))
{
    var jsonData = File.ReadAllText(samples);
    var loaded = JsonSerializer.Deserialize<List<Appointment>>(jsonData);
    if (loaded != null)
    {
        appointments.AddRange(loaded);
    }
}

app.MapGet("/appointments", () =>
{
    return Results.Ok(appointments);
});

app.MapGet("/appointments/{id}", (Guid id) =>
{
    var appointment = appointments.FirstOrDefault(a => a.Id == id);
    return appointment is not null ? Results.Ok(appointment) : Results.NotFound();
});

app.Run();