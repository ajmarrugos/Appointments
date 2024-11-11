using Microsoft.AspNetCore.Mvc;
using Appointments.API.Models;
using Appointments.API.Interfaces;
using Appointments.API.Repositories;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Logging;

namespace Appointments.API.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentsRepository _appointments;
        private readonly ILogger<AppointmentsController> _logger;
        private readonly List<string> _managers = new List<string> { "ajmarrugos@gmail.com" };

        // Dependency injection setting for the controller
        public AppointmentsController(IAppointmentsRepository appointmentsRepository, ILogger<AppointmentsController> logger)
        {
            _appointments = appointmentsRepository;
            _logger = logger;
        }

        [HttpGet] // GET: api/Appointments
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAllAppointments()
        {
            try
            {
                var appointments = await _appointments.GetAllAppointments();
                if (appointments == null)
                {
                    return NotFound("No appointments found.");
                }
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")] // GET: api/Appointments/{id}
        public async Task<ActionResult<Appointment>> GetAppointmentById(int id)
        {
            try
            {
                var appointment = await _appointments.GetAppointmentById(id);
                if (appointment == null)
                {
                    return NotFound($"Appointment with id {id} not found.");
                }
                return Ok(appointment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{attribute}/{value}")]
        public async Task<IActionResult> QueryAppointment(string attribute, string value) =>
            Ok(await _appointments.QueryAppointments(attribute, value));

        [HttpPost] // POST: api/Appointments
        public async Task<ActionResult<Appointment>> CreateAppointment([FromBody] Appointment appointment)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid appointment model received.");
                return BadRequest(ModelState);
            }

            try
            {
                var newAppointment = await _appointments.CreateAppointment(appointment);

                // Send notifications to both sender and recipient
                // SendEmailNotification(createdAppointment.SenderEmail, createdAppointment.RecipientEmail);

                return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("reschedule/{id}")] // PUT: api/Appointments/reschedule/{id}
        public async Task<ActionResult<Appointment>> RescheduleAppointment(int id, [FromBody] RescheduleRequest request)
        {
            var appointment = await _appointments.GetAppointmentById(id);
            if (appointment == null)
            {
                return NotFound("Appointment not found.");
            }

            if (appointment.Status != "pending")
            {
                return BadRequest("Only 'pending' appointments can be rescheduled.");
            }

            // Ensure the requestor is authorized to reschedule
            if (request.Sender != appointment.Sender && request.Sender != appointment.Recipient)
            {
                return Unauthorized("You are not authorized to reschedule this appointment.");
            }

            // Update appointment details
            appointment.Date = request.NewDate;
            appointment.Time = request.NewTime;
            appointment.Status = "Rescheduled";
            await _appointments.UpdateAppointment(appointment);

            return Ok($"Appointment '{appointment.Id}' sucessfully rescheduled for: {appointment.Date} @{appointment.Time}");
        }

        [HttpPut("sign/{senderEmail}/{id}")] // PUT: api/Appointments/signoff/{id}
        public async Task<ActionResult<Appointment>> SignAppointment(string senderEmail, int id, SignRequest signRequest)
        {
            try
            {
                var appointment = await _appointments.GetAppointmentById(id);
                if (appointment == null)
                    return NotFound("Appointment not found.");

                if (appointment.Status != "pending" && appointment.Status != "rescheduled")
                {
                    _logger.LogWarning("Attempt to sign an appointment that is not pending or rescheduled");
                    return BadRequest("Only 'pending' or 'rescheduled' appointments can be signed.");
                }

                if (appointment.Sender != signRequest.Sender && appointment.Recipient != signRequest.Sender)
                    return Unauthorized("You are not authorized to sign this appointment.");

                if (!_managers.Contains(signRequest.Sender))
                    return Unauthorized("Only managers are allowed to sign appointments.");

                appointment.Status = signRequest.Signature == "accepted" ? "approved" : "rejected";
                await _appointments.UpdateAppointment(appointment);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error signing appointment.");
                return StatusCode(500, "Internal server error while signing appointment.");
            }
        }

        [HttpDelete("delete/{senderEmail}/{id}")] // DELETE: api/Appointments/{id}
        public async Task<IActionResult> DeleteAppointment([FromQuery] int id, [FromQuery] string email)
        {
            // Retrieve the appointment by ID
            var appointment = await _appointments.GetAppointmentById(id);

            if (appointment == null)
                return NotFound("Appointment not found.");

            // Verify if the requestor is authorized
            if (appointment.Sender != email && appointment.Recipient != email)
                return Unauthorized("Only the requestor or recipient can remove this appointment.");

            if (!_managers.Contains(email))
                return Unauthorized("Only managers are allowed to remove appointments.");

            if (appointment.Status != "rejected" && appointment.Status != "expired")
                return BadRequest("Only 'rejected' or 'expired' appointments can be removed.");

            // Proceed with deletion
            await _appointments.DeleteAppointment(id);
            return NoContent();
        }

        [HttpPut("expire")] // PUT: api/Appointments/
        public async Task<IActionResult> ExpirePastAppointments()
        {
            var appointments = await _appointments.GetAllAppointments();
            var currentTime = DateTime.Now;

            foreach (var appointment in appointments)
            {
                if (appointment.Status == "created" &&
                    (appointment.Date < DateOnly.FromDateTime(currentTime) ||
                    (appointment.Date == DateOnly.FromDateTime(currentTime) &&
                     appointment.Time < TimeOnly.FromDateTime(currentTime))))
                {
                    appointment.Status = "expired";
                    await _appointments.UpdateAppointment(appointment);
                }
            }

            return NoContent();
        }
    }
}