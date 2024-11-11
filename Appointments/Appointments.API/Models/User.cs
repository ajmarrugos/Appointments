using System.ComponentModel.DataAnnotations;

namespace Appointments.API.Models
{
    public class User
    {
        [Key]
        public string Identifier { get; set; } // IP or Email

        [Required]
        public string Role { get; set; } // "Manager", "User"
    }
}
