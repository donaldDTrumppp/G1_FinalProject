using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Clinic_Management.Models;
using Clinic_Management.Utils;

namespace Clinic_Management.Pages.PatientAppointment
{
    public class DetailsModel : PageModel
    {
        private readonly Clinic_Management.Models.G1_PRJ_DBContext _context;

        private AppointmentBrotherCode _appointmentBrotherCode;

        public DetailsModel(Clinic_Management.Models.G1_PRJ_DBContext context)
        {
            _context = context;
        }

        public List<Branch> Branchs { get; set; }

        public List<Specialist> Specialists { get; set; }

        public bool Mode { get; set; } = false;

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; }

        [BindProperty(SupportsGet = true)]
        public Appointment Appointment { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id, int PageIndex, string Mde)
        {
            this.PageIndex = PageIndex;
            if (Mde != null)
            {
                if (Mde == "True")
                {
                    this.Mode = false;
                }
                else
                {
                    this.Mode = true;
                }

            }

            if (id == null || _context.Appointments == null)
            {
                return NotFound();
            }
            Branchs = await _context.Branches.Distinct().ToListAsync();
            Specialists = await _context.Specialists.Distinct().ToListAsync();

            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Receptionist)
                .Include(a => a.SpecialistNavigation)
                .Include(a => a.Branch)
                .Include(a => a.MedicalRecord)
                .Include(a => a.StatusNavigation)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
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

        public async Task<IActionResult> OnPostAsync()
        {
            Appointment.Branch = _context.Branches.FirstOrDefault(i => i.BranchId == Appointment.BranchId);
            Appointment.Patient = _context.Users.FirstOrDefault(i => i.UserId == Appointment.PatientId);
            Appointment.StatusNavigation = _context.AppointmentStatuses.FirstOrDefault(i => i.StatusId == Appointment.Status);
            if (!ModelState.IsValid)
            {
                
                return Page();
            }

            _context.Attach(Appointment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(Appointment.AppointmentId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }


            return RedirectToPage("./Index", new { PageIndex = this.PageIndex, Message = "Edit appointment successfully"});
        }

        private bool AppointmentExists(int id)
        {
            return (_context.Appointments?.Any(e => e.AppointmentId == id)).GetValueOrDefault();
        }

        public string GetCodeById()
        {
            _appointmentBrotherCode = new AppointmentBrotherCode(_context);
            return _appointmentBrotherCode.EncodeAppointment(Appointment);
        }
    }
}
