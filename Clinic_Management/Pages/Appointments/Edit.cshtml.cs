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
    public class EditModel : PageModel
    {
        private readonly Clinic_Management.Models.G1_PRJ_DBContext _context;

        public EditModel(Clinic_Management.Models.G1_PRJ_DBContext context)
        {
            _context = context;
        }

        public Appointment Appointment { get; set; } = default!;
        public User patient { get; set; } = default!;
        public IList<MedicalRecord> MedicalRecords { get; set; }

        public IList<User> DoctorList { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Appointments == null)
            {
                return NotFound();
            }

            var appointment= await _context.Appointments.Include(a=>a.Doctor).Include(a=>a.Branch)
                .Include(a=>a.SpecialistNavigation).Include(a=>a.StatusNavigation)
                .Include(a=>a.Doctor).Include(a=>a.Receptionist).FirstOrDefaultAsync(m => m.AppointmentId == id);
            patient = _context.Users.Include(p=>p.MedicalRecordPatients).FirstOrDefault(p => p.UserId == appointment.PatientId);
            MedicalRecords = _context.MedicalRecords.Where(m => m.PatientId == patient.UserId).ToList();
            if (appointment == null)
            {
                return NotFound();
            }
            else
            {
                Appointment = appointment;
            }
            List<User> Doctors = _context.Users.Include(s=>s.Staff).Where(s=>s.RoleId==2).ToList();
            List<User> DoctorToRemove = new List<User>();
            foreach (var user in Doctors) {
                foreach (var a in _context.Appointments.Where(a => a.Status == 1 && a.AppointmentId != appointment.AppointmentId).ToList()) {
                    if (a.RequestedTime.Equals(appointment.RequestedTime))
                    {
                        DoctorToRemove.Add(user);
                    }
                }
            }
            foreach(var u in DoctorToRemove)
            {
                foreach(var d in Doctors)
                {
                    if (u.UserId == d.UserId)
                    {
                        Doctors.Remove(u);
                    }
                }
            }
            DoctorList = Doctors.ToList();
            return Page();
        }
    }
}
