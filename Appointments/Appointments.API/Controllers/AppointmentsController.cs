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
        private readonly IRepository local;

        public AppointmentsController(IRepository local)
        {
            this.local = local;
        }

        // Endpoint to get all appointments
        [HttpGet]
        [OutputCache]
        public List<Appointment> Get()
        {
            var repo = new LocalRepository();
            var appointments = local.GetAppointments();
            return appointments;
        }

        // Endpoint to get a specific appointment by ID
        [HttpGet("{id:Guid}")]
        [OutputCache]
        public async Task<ActionResult<Appointment>> GetById(Guid id)
        {
            var repo = new LocalRepository();
            var appointment = await local.GetAppointmentsById(id);

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
            var appointment = await local.GetAppointmentsBySender(email);

            if (appointment is null)
            {
                return NotFound();
            }
            return appointment;
        }
    }
}