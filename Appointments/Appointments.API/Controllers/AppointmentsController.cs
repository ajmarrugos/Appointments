using Microsoft.AspNetCore.Mvc;
using Appointments.API.Models;
using Appointments.API.Interfaces;
using Appointments.API.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace Appointments.API.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentsRepository _appointmentsRepository;

        // Constructor dependency injection to receive the appropriate repository
        public AppointmentsController(IAppointmentsRepository appointmentsRepository)
        {
            _appointmentsRepository = appointmentsRepository;
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
        public async Task<ActionResult<Appointment>> GetAppointmentById(Guid id)
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

        [HttpGet("sender/{senderEmail}")] // GET: api/Appointments/sender/{email}
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointmentsBySender(string senderEmail)
        {
            try
            {
                var appointments = await _appointmentsRepository.GetAppointmentsBySender(senderEmail);
                if (appointments == null)
                {
                    return NotFound($"No appointments found for sender {senderEmail}.");
                }
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost] // POST: api/Appointments
        public async Task<ActionResult<Appointment>> CreateAppointment(Appointment appointment)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdAppointment = await _appointmentsRepository.CreateAppointment(appointment);

                // Send notifications to both sender and recipient (optional)
                // SendEmailNotification(createdAppointment.SenderEmail, createdAppointment.RecipientEmail);

                return CreatedAtAction(nameof(GetAppointmentById), new { id = createdAppointment.Id }, createdAppointment);
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("reschedule/{id}")] // PUT: api/Appointments/reschedule/{id}
        public async Task<ActionResult<Appointment>> RescheduleAppointment(Guid id, DateTime newDate)
        {
            try
            {
                // Validate the new date is in the future
                if (newDate <= DateTime.Now)
                {
                    return BadRequest("New appointment date must be in the future.");
                }

                var updatedAppointment = await _appointmentsRepository.RescheduleAppointment(id, newDate);
                if (updatedAppointment == null)
                {
                    return NotFound($"Appointment with id {id} not found or cannot be rescheduled.");
                }
                return Ok(updatedAppointment);
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("signoff/{id}")] // PUT: api/Appointments/signoff/{id}
        public async Task<ActionResult<Appointment>> SignoffAppointment(Guid id, AppointmentStatus status)
        {
            try
            {
                if (status != AppointmentStatus.Approved && status != AppointmentStatus.Canceled)
                {
                    return BadRequest("Only Approved or Canceled status can be applied.");
                }

                var updatedAppointment = await _appointmentsRepository.SignoffAppointment(id, status);
                if (updatedAppointment == null)
                {
                    return NotFound($"Appointment with id {id} not found or cannot be signed off.");
                }
                return Ok(updatedAppointment);
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")] // DELETE: api/Appointments/{id}
        public async Task<ActionResult> DeleteAppointment(Guid id)
        {
            try
            {
                var isDeleted = await _appointmentsRepository.DeleteAppointment(id);
                if (!isDeleted)
                {
                    return NotFound($"Appointment with id {id} not found or cannot be deleted.");
                }
                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}