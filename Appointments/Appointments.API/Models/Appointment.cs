namespace Appointments.API.Models
{
    public record Appointment
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string SenderEmail { get; set; } = string.Empty;
        public string RecipientEmail { get; set; } = string.Empty;
        public string ApptName { get; set; } = string.Empty;
        public DateTime ApptDate { get; set; }
    }
}