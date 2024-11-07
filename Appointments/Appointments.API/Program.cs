var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/appointments", () => "Here is a list of appointments");

app.Run();