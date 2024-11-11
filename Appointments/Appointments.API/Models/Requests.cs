using System.ComponentModel.DataAnnotations;

namespace Appointments.API.Models
{
    // DTO for rescheduling request
    public class RescheduleRequest
    {
        [Required(ErrorMessage = "Sender email is required.")]
        [EmailAddress(ErrorMessage = "Invalid sender email format.")]
        public string Sender { get; set; }

        [Required(ErrorMessage = "Appointment date is required")]
        [ValidDate(ErrorMessage = "Appointment date must be in  YY-MM-DD format and cannot be past")]
        public DateOnly NewDate { get; set; }

        [Required(ErrorMessage = "Appointment time is required ")]
        public TimeOnly NewTime { get; set; }
    }

    // DTO for signing request
    public class SignRequest
    {
        public string Sender { get; set; }
        public string Signature { get; set; } // Expected to be "approved" or "rejected"
    }
}
