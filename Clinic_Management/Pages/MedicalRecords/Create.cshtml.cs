using Clinic_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Clinic_Management.Pages.MedicalRecords
{
    public class CreateModel : PageModel
    {
        private readonly Clinic_Management.Models.G1_PRJ_DBContext _context;

        public CreateModel(Clinic_Management.Models.G1_PRJ_DBContext context)
        {
            _context = context;
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
                int TotalRecord = _context.MedicalRecords.ToList().Count;
                int PageIndex = TotalRecord % 5 == 0 ? TotalRecord / 5 : (TotalRecord / 5 + 1);
                return RedirectToPage("./Index", new { PageIndex = PageIndex, TypeMessage = "success", Message = "Create appointment successfully" });
            } catch(Exception e)
            {
                return RedirectToPage("./Create", new { TypeMessage = "error",  Message = "Create appointment fail: " + e.Message });
            }
        }
    }
}
