using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Clinic_Management.Models;
using Microsoft.AspNetCore.Authorization;

namespace Clinic_Management.Pages.Appointments
{
    [Authorize(Policy = "StaffPolicy")]
    public class DetailsModel : PageModel
    {
        private readonly Clinic_Management.Models.G1_PRJ_DBContext _context;
        private readonly Clinic_Management.Utils.Authentication authentication;
        private readonly IConfiguration _configuration;
        public DetailsModel(Clinic_Management.Models.G1_PRJ_DBContext context, IConfiguration config)
        {
            _context = context;
            _configuration = config;
            authentication = new Clinic_Management.Utils.Authentication(context, config);
        }

        public Appointment Appointment { get; set; } = default!;
        public User patient { get; set; } = default!;
        public IList<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public string error {  get; set; }
        public string errorMessage {  get; set; }
        public int roleID {  get; set; }
        public async Task<IActionResult> OnGetAsync(int? id,string error,string errorMessage)
        {
            string token = HttpContext.Request.Cookies["AuthToken"];
            User user = authentication.GetUserFromToken(token);
            roleID = user.RoleId;
            this.error = error;
            this.errorMessage = errorMessage;

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
        public IActionResult OnGetAssignMe(int id)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == id);
            string token = HttpContext.Request.Cookies["AuthToken"];
            User user = authentication.GetUserFromToken(token);
            roleID = user.RoleId;
            var doctor = _context.Staff.Include(d => d.DoctorDepartment).FirstOrDefault(x => x.UserId == user.UserId);
            bool isError = false;
            if (appointment == null || doctor == null)
            {
                return NotFound();
            }

            if (appointment.BranchId != doctor.DoctorDepartmentId)
            {
                error += "You are not working on this branch. ";
                isError = true;
            }

            if (appointment.Specialist != doctor.DoctorSpecialist)
            {
                error += "This is not your specialist. ";
                isError = true;
            }

            var conflictingAppointments = _context.Appointments
                .Where(a => a.DoctorId == user.UserId && a.RequestedTime.Equals(appointment.RequestedTime) && (a.Status == 1 || a.Status == 4))
                .ToList();

            if (conflictingAppointments.Count > 0)
            {
                error += "You already have an appointment at this time. ";
                isError = true;
            }
            if (isError)
            {
                errorMessage = "Error occurred!";
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                return RedirectToPage("/Appointments/Details", new { id = appointment.AppointmentId , error = error,errorMessage=errorMessage});
            }
            else
            {
                appointment.DoctorId = user.UserId;
                _context.SaveChanges();
                return RedirectToPage("/Appointments/Index", new { Message = "Appointment updated!" });
            }
            
        }
    }
}
