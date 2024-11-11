using Microsoft.EntityFrameworkCore;
using Appointments.API.Models;

namespace Appointments.API.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Appointment> Appointments { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointment>()
                .ToTable("Appointments")
                .HasKey(a => a.Id);
        }
    }
}