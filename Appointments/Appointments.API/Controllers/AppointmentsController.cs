using Microsoft.AspNetCore.Mvc;
using Appointments.API.Models;
using Appointments.API.Repository;
using Microsoft.AspNetCore.OutputCaching;

namespace Appointments.API.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    public class AppointmentsController : ControllerBase
    {
        // Endpoint to get all appointments
        [HttpGet]
        [OutputCache]
        public List<Appointment> Get()
        {
            var repo = new LocalRepository();
            var appointments = repo.GetAppointments();
            return appointments;
        }

        // Endpoint to get a specific appointment by ID
        [HttpGet("{id:Guid}")]
        [OutputCache]
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
        [OutputCache]
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