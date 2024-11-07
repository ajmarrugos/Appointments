using Appointments.API.Models;

namespace Appointments.API.Repository
{
    public class LocalRepository
    {
        /// <summary>
        /// In-memory list to store appointments (acts as a temporary DB).
        /// </summary>
        private List<Appointment> _appointments;

        /// <summary>
        /// Instance constructor for a local repository
        /// </summary>
        public LocalRepository()
        {
            _appointments = new List<Appointment>
            {
                new Appointment {
                    SenderEmail = "ajmarrugos@gmail.com",
                    RecipientEmail = "alberto.marrugo@unosquare.com",
                    ApptName = "Alberto's Assesment",
                    ApptDate = DateTime.Now,
                    Status = AppointmentStatus.Created
                },
            };
        }

        /// <summary>
        /// Method to get a list of appointments
        /// </summary>
        /// <returns>List of appointments</returns>
        public List<Appointment> GetAppointments()
        {
            return _appointments;
        }

        /// <summary>
        /// Method to get appointments by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>An specific appointment</returns>
        public Appointment? GetAppointmentsById(Guid id)
        {
            return _appointments.FirstOrDefault(a => a.Id == id);
        }

        /// <summary>
        /// Method to get appointments by Sender Email
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Appointments by Sender</returns>
        public Appointment? GetAppointmentsBySender(string email)
        {
            return _appointments.FirstOrDefault(a => a.SenderEmail == email);
        }
    }
}
