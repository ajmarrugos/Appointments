using Microsoft.AspNetCore.Mvc;
using Appointments.API.Models;

namespace Appointments.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        // In-memory list to store appointments 
        private static readonly List<Appointment> Appointments = new();

        // GET: api/appointments - Endpoint to get all appointments
        [HttpGet]
        public IActionResult GetAppointments()
        {
            return Ok(Appointments);
        }

        // GET: api/appointments/{id} - Endpoint to get a specific appointment by ID
        [HttpGet("{id}")]
        public IActionResult GetAppointmentById(Guid id)
        {
            var appointment = Appointments.FirstOrDefault(a => a.Id == id);
            return appointment is not null ? Ok(appointment) : NotFound();
        }

        // POST: api/appointments - Endpoint to create a new appointment
        [HttpPost]
        public IActionResult CreateAppointment([FromBody] Appointment appointment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Appointments.Add(appointment);
            return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, appointment);
        }
    }
}