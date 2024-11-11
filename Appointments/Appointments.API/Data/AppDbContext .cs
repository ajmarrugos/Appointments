using Microsoft.EntityFrameworkCore;
using Appointments.API.Models;

namespace Appointments.API.Data
{
    public class AppDbContext : DbContext
    {
        // DbSet for Appointments table
        public DbSet<Appointment> Appointments { get; set; }

        // DbSet for Users table
        public DbSet<User> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the Appointments table
            modelBuilder.Entity<Appointment>()
                .ToTable("Appointments")
                .HasKey(a => a.Id);

            modelBuilder.Entity<Appointment>()
                .Property(a => a.Status)
                .HasMaxLength(50);

            // Configure the Users table
            modelBuilder.Entity<User>()
                .ToTable("Users")
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}