using Microsoft.AspNetCore.Mvc;
using Appointments.API.Models;

namespace Appointments.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private static readonly List<Appointment> Appointments = new();

        [HttpGet]
        public IActionResult GetAppointments()
        {
            return Ok(Appointments);
        }

        [HttpGet("{id}")]
        public IActionResult GetAppointmentById(Guid id)
        {
            var appointment = Appointments.FirstOrDefault(a => a.Id == id);
            return appointment is not null ? Ok(appointment) : NotFound();
        }
    }
}