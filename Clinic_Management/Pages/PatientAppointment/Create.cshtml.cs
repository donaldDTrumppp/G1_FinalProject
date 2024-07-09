using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Clinic_Management.Models;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Management.Pages.PatientAppointment
{
    public class CreateModel : PageModel
    {
        private readonly Clinic_Management.Models.G1_PRJ_DBContext _context;

        public CreateModel(Clinic_Management.Models.G1_PRJ_DBContext context)
        {
            _context = context;
        }

        public List<Branch> Branchs { get; set; }

        public List<Specialist> Specialists { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Branchs = await _context.Branches.Distinct().ToListAsync();
            Specialists = await _context.Specialists.Distinct().ToListAsync();
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
            Appointment.Status = _context.AppointmentStatuses.FirstOrDefault(i => i.StatusName == "Đang chờ duyệt").StatusId;
            Appointment.Branch = _context.Branches.FirstOrDefault(i => i.BranchId == Appointment.BranchId);
            Appointment.CreatedAt = DateTime.Now;
            if (!ModelState.IsValid || _context.Appointments == null || Appointment == null)
            {
                return Page();
            }

            int TotalRecord = _context.Appointments.ToList().Count;
            int PageIndex = TotalRecord % 5 == 0 ? TotalRecord / 5 : TotalRecord / 5 + 1;

            _context.Appointments.Add(Appointment);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index", new { PageIndex = PageIndex, Message = "Create appointment request successfully" });
        }
    }
}
