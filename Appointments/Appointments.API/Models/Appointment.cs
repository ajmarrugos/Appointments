using System.ComponentModel.DataAnnotations;

namespace Appointments.API.Models
{
    public class Appointment
    {
        // Automatically generates a new Guid for each appointment.
        public Guid Id { get; init; } = Guid.NewGuid();

        // SenderEmail must be provided and must be in a valid email format.
        [Required(ErrorMessage = "Sender email is required.")]
        [EmailAddress(ErrorMessage = "Invalid sender email format.")]
        public string SenderEmail { get; set; } = string.Empty;

        // RecipientEmail must be provided and must be in a valid email format.
        [Required(ErrorMessage = "Recipient email is required.")]
        [EmailAddress(ErrorMessage = "Invalid recipient email format.")]
        public string RecipientEmail { get; set; } = string.Empty;

        // ApptName must be provided and cannot be empty.
        [Required(ErrorMessage = "Appointment name is required.")]
        public string ApptName { get; set; } = string.Empty;

        // ApptDate must be a valid date in the future.
        [Required(ErrorMessage = "Appointment date is required.")]
        [FutureDate(ErrorMessage = "Appointment date must be in the future.")]
        public DateTime ApptDate { get; set; }
    }

    // Custom Validation Attribute for validating that the appointment date is in the future
    public class FutureDateAttribute : ValidationAttribute
    {
        // Checks if the value (appointment date) is greater than the current date and time
        public override bool IsValid(object? value)
        {
            if (value is DateTime dateTime)
            {
                return dateTime > DateTime.Now;  // Ensure date is in the future
            }
            return false;
        }
    }
}