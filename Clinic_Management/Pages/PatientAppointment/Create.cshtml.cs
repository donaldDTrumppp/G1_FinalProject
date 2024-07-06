using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Clinic_Management.Models;

namespace Clinic_Management.Pages.PatientAppointment
{
    public class CreateModel : PageModel
    {
        private readonly Clinic_Management.Models.G1_PRJ_DBContext _context;

        public CreateModel(Clinic_Management.Models.G1_PRJ_DBContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["BranchId"] = new SelectList(_context.Branches, "BranchId", "BranchId");
        ViewData["DoctorId"] = new SelectList(_context.Users, "UserId", "UserId");
        ViewData["PatientId"] = new SelectList(_context.Users, "UserId", "UserId");
        ViewData["ReceptionistId"] = new SelectList(_context.Users, "UserId", "UserId");
        ViewData["Specialist"] = new SelectList(_context.Specialists, "SpecialistId", "SpecialistId");
        ViewData["Status"] = new SelectList(_context.AppointmentStatuses, "StatusId", "StatusId");
            return Page();
        }

        [BindProperty]
        public Appointment Appointment { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Appointments == null || Appointment == null)
            {
                return Page();
            }

            _context.Appointments.Add(Appointment);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
