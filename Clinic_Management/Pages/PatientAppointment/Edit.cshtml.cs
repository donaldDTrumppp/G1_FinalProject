using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Clinic_Management.Models;
using Clinic_Management.Services;

namespace Clinic_Management.Pages.PatientAppointment
{
    public class EditModel : PageModel
    {
        private readonly Clinic_Management.Models.G1_PRJ_DBContext _context;

        private readonly IConfiguration _config;

        private readonly NotificationService _notificationService;

        public EditModel(Clinic_Management.Models.G1_PRJ_DBContext context, IConfiguration config, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        [BindProperty]
        public Appointment Appointment { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Appointments == null)
            {
                return NotFound();
            }

            var appointment =  await _context.Appointments.FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }
            Appointment = appointment;
           ViewData["BranchId"] = new SelectList(_context.Branches, "BranchId", "BranchId");
           ViewData["DoctorId"] = new SelectList(_context.Users, "UserId", "UserId");
           ViewData["PatientId"] = new SelectList(_context.Users, "UserId", "UserId");
           ViewData["ReceptionistId"] = new SelectList(_context.Users, "UserId", "UserId");
           ViewData["Specialist"] = new SelectList(_context.Specialists, "SpecialistId", "SpecialistId");
           ViewData["Status"] = new SelectList(_context.AppointmentStatuses, "StatusId", "StatusId");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
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

            return RedirectToPage("./Index");
        }

        private bool AppointmentExists(int id)
        {
          return (_context.Appointments?.Any(e => e.AppointmentId == id)).GetValueOrDefault();
        }
    }
}
