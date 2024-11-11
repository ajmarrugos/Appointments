using System.ComponentModel.DataAnnotations;

namespace Appointments.API.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; } // In a real application, this should be hashed

        [Required]
        public string Role { get; set; } // "Manager", "User"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
