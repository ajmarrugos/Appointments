using Microsoft.EntityFrameworkCore;
using Appointments.API.Models;

namespace Appointments.API.Data
{
    public class AppDbContext : DbContext
    {
        // Defines a DbSet properties for each entity in your database
        public DbSet<Appointment> Appointments { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Optionally override OnModelCreating to configure model relationships
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Additional model configuration can be added here
        }
    }
}