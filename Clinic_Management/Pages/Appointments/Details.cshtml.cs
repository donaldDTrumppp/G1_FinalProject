using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Clinic_Management.Models;

namespace Clinic_Management.Pages.Appointements
{
    public class DetailsModel : PageModel
    {
        private readonly Clinic_Management.Models.G1_PRJ_DBContext _context;

        public DetailsModel(Clinic_Management.Models.G1_PRJ_DBContext context)
        {
            _context = context;
        }

        public Appointment Appointment { get; set; } = default!;
        public User patient { get; set; } = default!;
        public IList<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Appointments == null)
            {
                return NotFound();
            }

            var appointment= await _context.Appointments.Include(a=>a.Doctor).Include(a=>a.Branch).Include(p=>p.Patient)
                .Include(a=>a.SpecialistNavigation).Include(a=>a.StatusNavigation)
                .Include(a=>a.Doctor).Include(a=>a.Receptionist).FirstOrDefaultAsync(m => m.AppointmentId == id);
            patient = _context.Users.Include(p=>p.Patient).Include(p=>p.Patient.MedicalRecords).FirstOrDefault(p => p.UserId == appointment.PatientId);
            if (patient != null)
            {
                MedicalRecords = _context.MedicalRecords.Where(m => m.PatientId == patient.UserId).ToList();
            }
            
            if (appointment == null)
            {
                return NotFound();
            }
            else
            {
                Appointment = appointment;
            }
            return Page();
        }
    }
}
