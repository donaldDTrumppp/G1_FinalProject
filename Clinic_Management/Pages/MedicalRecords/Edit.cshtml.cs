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
using Microsoft.AspNetCore.Authorization;

namespace Clinic_Management.Pages.MedicalRecords
{
    [Authorize(Policy = "DoctorPolicy")]
    public class EditModel : PageModel
    {
        private readonly Clinic_Management.Models.G1_PRJ_DBContext _context;

        private readonly EmailService _emailService;

        private readonly NotificationService _notificationService;

        private readonly IConfiguration _config;
        public EditModel(Clinic_Management.Models.G1_PRJ_DBContext context, EmailService emailService, IConfiguration config, NotificationService notificationService)
        {
            _context = context;
            _emailService = emailService;
            _config = config;
            _notificationService = notificationService;
        }

        [BindProperty]
        public int? PageIndex { get; set; } = 1;
        [BindProperty]
        public MedicalRecord MedicalRecord { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id, int? PageIndex)
        {
            this.PageIndex = PageIndex;
            if (id == null || _context.MedicalRecords == null)
            {
                return RedirectToPage("/Home/404");
            }

            var medicalrecord = await _context.MedicalRecords
                .Include(m => m.Appointment)
                .Include(d => d.Doctor).ThenInclude(s => s.Staff).ThenInclude(d => d.DoctorSpecialistNavigation)
                .FirstOrDefaultAsync(m => m.MedicalrecordId == id);

            if (medicalrecord == null)
            {
                return RedirectToPage("/Home/404");
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
            catch (Exception)
            {
                if (!MedicalRecordExists(MedicalRecord.MedicalrecordId))
                {
                    return RedirectToPage("/Home/404");
                }
                else
                {
                    throw;
                }
            }

            int id = MedicalRecord.MedicalrecordId;

            MedicalRecord = _context.MedicalRecords
                .Include(a => a.Appointment).ThenInclude(b => b.Branch)
                .Include(d => d.Doctor)
                .FirstOrDefault(a => a.MedicalrecordId == id);

            int TotalRecord = _context.MedicalRecords.ToList().Count;
            int PageIndex = TotalRecord % 5 == 0 ? TotalRecord / 5 : (TotalRecord / 5 + 1);

            string activeLinkNoti = _config["Host"] + _config["Port"] + "/MedicalRecords/Details?id=" + MedicalRecord.MedicalrecordId;
            await _notificationService.SendMedicalRecordNotification(MedicalRecord.PatientId.GetValueOrDefault(), $"A medical record has EDITED by doctor {MedicalRecord.Doctor.Name}", activeLinkNoti);

            string activeLink = _config["Host"] + _config["Port"] + "/MedicalRecords/Details?id=" + MedicalRecord.MedicalrecordId;
            var htmlContent = await _emailService.GetMedicalRecordEmail("medical_record_edited.html",
            MedicalRecord.Appointment.Branch.BranchName,
            MedicalRecord.Appointment.PatientName,
            MedicalRecord.Appointment.PatientAddress,
            MedicalRecord.Appointment.PatientDob.ToString(),
            MedicalRecord.Appointment.PatientPhoneNumber,
            MedicalRecord.Appointment.PatientEmail,
            MedicalRecord.Symptoms,
            MedicalRecord.Diagnosis,
            MedicalRecord.Treatment,
            activeLink,
            MedicalRecord.Doctor?.Name ?? "N/A",
            MedicalRecord.VisitTime);
            _emailService.SendEmailMedical(MedicalRecord.Appointment.PatientEmail,
                "[Medical Record] Medical Record Edited Successfully",
                htmlContent);

            return RedirectToPage("./Index", new { PageIndex = this.PageIndex, Message = "Edit Medical report successfully" });
        }

        private bool MedicalRecordExists(int id)
        {
            return (_context.MedicalRecords?.Any(e => e.MedicalrecordId == id)).GetValueOrDefault();
        }
    }
}
