using System.Text.Json;
using Appointments.API.Models;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

var appointments = new List<Appointment>();

app.MapGet("/appointments", () =>
{
    return Results.Ok(appointments);
});

app.Run();