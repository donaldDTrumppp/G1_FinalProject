using Clinic_Management.Models;
using Clinic_Management.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Twilio.TwiML.Voice;

namespace Clinic_Management.Pages.MedicalRecords
{

    [Authorize(Policy = "DoctorPolicy")]

    public class CreateModel : PageModel
    {
        private readonly Clinic_Management.Models.G1_PRJ_DBContext _context;

        private readonly EmailService _emailService;

        private readonly IConfiguration _config;
        public CreateModel(Clinic_Management.Models.G1_PRJ_DBContext context, EmailService emailService, IConfiguration config)
        {
            _context = context;
            _emailService = emailService;
            _config = config;
        }

        public string Message { get; set; } = "";
        public string TypeMessage { get; set; } = "";

        public IActionResult OnGet(string? TypeMessage, string? Message)
        {
            this.TypeMessage = TypeMessage;
            this.Message = Message;
            ViewData["AppointmentId"] = new SelectList(_context.Appointments, "AppointmentId", "AppointmentId");
            ViewData["DoctorId"] = new SelectList(_context.Users, "UserId", "UserId");
            ViewData["PatientId"] = new SelectList(_context.Users, "UserId", "UserId");
            return Page();
        }

        [BindProperty]
        public MedicalRecord MedicalRecord { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.MedicalRecords == null || MedicalRecord == null)
            {
                return Page();
            }
            MedicalRecord.CreatedAt = DateTime.Now;
            _context.MedicalRecords.Add(MedicalRecord);
            try
            {
                await _context.SaveChangesAsync();
                int id = MedicalRecord.MedicalrecordId;

                MedicalRecord = _context.MedicalRecords
                    .Include(a => a.Appointment).ThenInclude(b => b.Branch)
                    .Include(d => d.Doctor)
                    .FirstOrDefault(a => a.MedicalrecordId == id);

                int TotalRecord = _context.MedicalRecords.ToList().Count;
                int PageIndex = TotalRecord % 5 == 0 ? TotalRecord / 5 : (TotalRecord / 5 + 1);

                string activeLink = _config["Host"] + _config["Port"] + "/MedicalRecords/Details?id=" + MedicalRecord.MedicalrecordId;
                var htmlContent = await _emailService.GetMedicalRecordEmail("medical_record_created.html", 
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
                    "[Medical Record] Medical Record Created Successfully",
                    htmlContent);
               
                return RedirectToPage("./Index", new { PageIndex = PageIndex, TypeMessage = "success", Message = "Create appointment successfully" });
            } catch(Exception e)
            {
                return RedirectToPage("./Create", new { TypeMessage = "error",  Message = "Create appointment fail: " + e.Message });
            }
        }
    }
}
