using Microsoft.AspNetCore.Mvc;
using Appointments.API.Models;
using Appointments.API.Repository;

namespace Appointments.API.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    public class AppointmentsController : ControllerBase
    {
        // Endpoint to get all appointments
        [HttpGet]
        public List<Appointment> Get()
        {
            var repo = new LocalRepository();
            var appointments = repo.GetAppointments();
            return appointments;
        }

        // Endpoint to get a specific appointment by ID
        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<Appointment>> GetById(Guid id)
        {
            var repo = new LocalRepository();
            var appointment = await repo.GetAppointmentsById(id);

            if (appointment is null)
            {
                return NotFound();
            }
            return appointment;
        }

        // Endpoint to get a specific appointment by Sender Email
        [HttpGet("{sender}")]
        public async Task<ActionResult<Appointment>> GetBySender(string email)
        {
            var repo = new LocalRepository();
            var appointment = await repo.GetAppointmentsBySender(email);

            if (appointment is null)
            {
                return NotFound();
            }
            return appointment;
        }
    }
}