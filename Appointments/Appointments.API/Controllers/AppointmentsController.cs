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
        private readonly IAppointmentsRepository _appointmentsRepository;
        private readonly ILogger<AppointmentsController> _logger;
        private readonly List<string> _managers = new List<string> { "ajmarrugos@gmail.com" };

        // Dependency injection setting for the controller
        public AppointmentsController(IAppointmentsRepository appointmentsRepository, ILogger<AppointmentsController> logger)
        {
            _appointmentsRepository = appointmentsRepository;
            _logger = logger;
        }

        [HttpGet] // GET: api/Appointments
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAllAppointments()
        {
            try
            {
                var appointments = await _appointmentsRepository.GetAllAppointments();
                if (appointments == null)
                {
                    return NotFound("No appointments found.");
                }
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")] // GET: api/Appointments/{id}
        public async Task<ActionResult<Appointment>> GetAppointmentById(int id)
        {
            try
            {
                var appointment = await _appointmentsRepository.GetAppointmentById(id);
                if (appointment == null)
                {
                    return NotFound($"Appointment with id {id} not found.");
                }
                return Ok(appointment);
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

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
                var newAppointment = await _appointmentsRepository.CreateAppointment(appointment);

                // Send notifications to both sender and recipient
                // SendEmailNotification(createdAppointment.SenderEmail, createdAppointment.RecipientEmail);

                await _appointmentsRepository.CreateAppointment(appointment);
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
            var appointment = await _appointmentsRepository.GetAppointmentById(id);
            if (appointment == null)
            {
                return NotFound("Appointment not found.");
            }

            if (appointment.Status != "Created")
            {
                return BadRequest("Only appointments in 'Created' status can be rescheduled.");
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
            await _appointmentsRepository.UpdateAppointment(appointment);

            return NoContent();
        }

        [HttpPut("signoff/{senderEmail}/{id}")] // PUT: api/Appointments/signoff/{id}
        public async Task<ActionResult<Appointment>> SignAppointment(string senderEmail, int id, SignRequest signRequest)
        {
            try
            {
                var appointment = await _appointmentsRepository.GetAppointmentById(id);
                if (appointment == null)
                    return NotFound("Appointment not found.");

                if (appointment.Status != "Created" && appointment.Status != "Rescheduled")
                {
                    _logger.LogWarning("Attempt to sign an appointment not in Created or Rescheduled state.");
                    return BadRequest("Only appointments in 'Created' or 'Rescheduled' state can be signed.");
                }

                if (appointment.Sender != signRequest.Sender && appointment.Recipient != signRequest.Sender)
                    return Unauthorized("You are not authorized to sign this appointment.");

                if (!_managers.Contains(signRequest.Sender))
                    return Unauthorized("Only managers are allowed to sign appointments.");

                appointment.Status = signRequest.Signature == "Accepted" ? "Approved" : "Rejected";
                await _appointmentsRepository.UpdateAppointment(appointment);
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
            var appointment = await _appointmentsRepository.GetAppointmentById(id);

            if (appointment == null)
                return NotFound("Appointment not found.");

            // Verify if the requestor is authorized
            if (appointment.Sender != email && appointment.Recipient != email)
                return Unauthorized("Only the requestor or recipient can remove this appointment.");

            if (!_managers.Contains(email))
                return Unauthorized("Only managers are allowed to remove appointments.");

            if (appointment.Status != "Denied" && appointment.Status != "Expired")
                return BadRequest("Only 'Denied' or 'Expired' appointments can be removed.");

            // Proceed with deletion
            await _appointmentsRepository.DeleteAppointment(id);
            return NoContent();
        }
    }
}