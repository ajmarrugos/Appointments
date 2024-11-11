namespace Appointments.API.Models
{
    // DTO for rescheduling request
    public class RescheduleRequest
    {
        public string Sender { get; set; }
        public DateOnly NewDate { get; set; }
        public TimeOnly NewTime { get; set; }
    }

    // DTO for signing request
    public class SignRequest
    {
        public string Sender { get; set; }
        public string Signature { get; set; } // Expected to be "Accepted" or "Rejected"
    }
}
