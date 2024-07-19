using Clinic_Management.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Clinic_Management.Pages.PatientAppointment
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientAppointmentController : Controller
    {
        private readonly G1_PRJ_DBContext _context;

        public PatientAppointmentController(G1_PRJ_DBContext context)
        {
            _context = context;
        }

        [HttpPut("{appointmentId}")]
        public async Task<ActionResult<Appointment>> PostPatientAppointment(int appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
            {
                return NotFound();
            }

            // Update the status to 3
            appointment.Status = 3;
            await _context.SaveChangesAsync();

            return Ok(appointment);
        }
    }
}
