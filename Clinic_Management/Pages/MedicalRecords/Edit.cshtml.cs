using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Clinic_Management.Models;

namespace Clinic_Management.Pages.MedicalRecords
{
    public class EditModel : PageModel
    {
        private readonly G1_PRJ_DBContext _context;

        public EditModel(G1_PRJ_DBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MedicalRecord MedicalRecord { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.MedicalRecords == null)
            {
                return NotFound();
            }

            var medicalrecord = await _context.MedicalRecords
                .Include(m => m.Appointment)
                .Include(d => d.Doctor).ThenInclude(s => s.Staff).ThenInclude(d => d.DoctorSpecialistNavigation)
                .FirstOrDefaultAsync(m => m.MedicalrecordId == id);

            if (medicalrecord == null)
            {
                return NotFound();
            }
            MedicalRecord = medicalrecord;
            ViewData["AppointmentId"] = new SelectList(_context.Appointments, "AppointmentId", "AppointmentId");
            ViewData["DoctorId"] = new SelectList(_context.Users, "UserId", "UserId");
            ViewData["PatientId"] = new SelectList(_context.Users, "UserId", "UserId");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    if (state.Value.Errors.Count > 0)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            // You can use any logging mechanism, here just for demonstration
                            System.Diagnostics.Debug.WriteLine($"Field: {state.Key}, Error: {error.ErrorMessage}");
                        }
                    }
                }
                return Page();
            }
            if(MedicalRecord.CreatedAt is null) MedicalRecord.CreatedAt = DateTime.Now;
            _context.Attach(MedicalRecord).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedicalRecordExists(MedicalRecord.MedicalrecordId))
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

        private bool MedicalRecordExists(int id)
        {
            return (_context.MedicalRecords?.Any(e => e.MedicalrecordId == id)).GetValueOrDefault();
        }
    }
}
