using System;
using System.ComponentModel.DataAnnotations;

namespace Appointments.API.Models
{
    // Appointment class 
    public class Appointment
    {
        // Key indexer / identifier assigned for each appointment
        [Key]
        public int Id { get; set; }

        // SenderEmail must be provided and must be in a valid email format
        [Required(ErrorMessage = "Sender email is required.")]
        [EmailAddress(ErrorMessage = "Invalid sender email format.")]
        public string Sender { get; set; } = string.Empty;

        // RecipientEmail must be provided and must be in a valid email format
        [Required(ErrorMessage = "Recipient email is required.")]
        [EmailAddress(ErrorMessage = "Invalid recipient email format.")]
        public string Recipient { get; set; } = string.Empty;

        // ApptName must be provided and cannot be empty and must be 48 characters only
        [Required(ErrorMessage = "Appointment name is required. No more than 48 characters"), StringLength(48)]
        public string Name { get; set; } = string.Empty;

        // ApptDate must be a valid date in the future
        [Required(ErrorMessage = "Appointment date is required")]
        [ValidDate(ErrorMessage = "Appointment date must be in  YY-MM-DD format and cannot be past")]
        public DateOnly Date { get; set; }

        // ApptTime must be a valid time in the future
        [Required(ErrorMessage = "Appointment time is required ")]
        [ValidTime(ErrorMessage = "Appointment date must be provided in HH:MM format and cannot be past")]
        public TimeOnly Time { get; set; }

        // Status of the appointment (Created, Approved, Rescheduled, Canceled)
        [Required(ErrorMessage = "Appointment status is required.")]
        [ValidStatus(ErrorMessage = "Appointment status must be valid")]
        public string Status { get; set; } = "created";
    }

    // Custom Validation Attribute for validating that the appointment date is in the future
    public class ValidDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateOnly date)
            {
                if (date >= DateOnly.FromDateTime(DateTime.Now))
                {
                    return ValidationResult.Success;
                }
                return new ValidationResult("Date cannot be in the past.");
            }
            return new ValidationResult("Invalid date format.");
        }
    }

    // Custom Validation Attribute for validating that the appointment time is valid format
    public class ValidTimeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is TimeOnly time)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("Invalid time format.");
        }
    }

    // Custom Validation Attribute for validating that the appointment status is valid
    public class ValidStatusAttribute : ValidationAttribute
    {
        private readonly string[] _validStatuses = { "created", "approved", "rescheduled", "rejected", "expired", "removed" };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string status && Array.Exists(_validStatuses, s => s.Equals(status, StringComparison.OrdinalIgnoreCase)))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult($"Invalid status. Allowed statuses are: {string.Join(", ", _validStatuses)}.");
        }
    }
}
