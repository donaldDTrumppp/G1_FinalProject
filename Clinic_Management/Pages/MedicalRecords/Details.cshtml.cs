using Clinic_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Clinic_Management.Pages.MedicalRecords.utils;
using Xceed.Words.NET;

namespace Clinic_Management.Pages.MedicalRecords
{
    public class DetailsModel : PageModel
    {
        private readonly Clinic_Management.Models.G1_PRJ_DBContext _context;
        private AppointmentBrotherCode brotherCode;

        public DetailsModel(Clinic_Management.Models.G1_PRJ_DBContext context)
        {
            _context = context;
            brotherCode = new AppointmentBrotherCode(_context);
        }

        public MedicalRecord MedicalRecord { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.MedicalRecords == null)
            {
                return RedirectToPage("/Home/404");
            }

            var medicalrecord = await _context.MedicalRecords
                .Include(s => s.Doctor).ThenInclude(d => d.Staff).ThenInclude(s => s.DoctorSpecialistNavigation)
                .Include(s => s.Patient)
                .FirstOrDefaultAsync(m => m.MedicalrecordId == id);
            if (medicalrecord == null)
            {
                return RedirectToPage("/Home/404");
            }
            else
            {
                MedicalRecord = medicalrecord;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.MedicalRecords == null)
            {
                return RedirectToPage("/Home/404");
            }

            var medicalrecord = await _context.MedicalRecords
                .Include(s => s.Doctor).ThenInclude(d => d.Staff).ThenInclude(s => s.DoctorSpecialistNavigation)
                .Include(s => s.Patient)
                .FirstOrDefaultAsync(m => m.MedicalrecordId == id);
            if (medicalrecord == null)
            {
                return RedirectToPage("/Home/404");
            }
            else
            {
                MedicalRecord = medicalrecord;
            }

            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/template_report.docx");

            using (var document = DocX.Load(templatePath))
            {
                // Replace placeholders with actual data
                document.ReplaceText("{DoctorName}", MedicalRecord.Doctor != null? MedicalRecord.Doctor.Name : "N/A");
                document.ReplaceText("{SpecialistName}", MedicalRecord.Doctor != null ? MedicalRecord.Doctor.Staff.DoctorSpecialistNavigation.SpecialistName : "N/A");
                document.ReplaceText("{PatientName}", MedicalRecord.Patient.Name);
                document.ReplaceText("{PatientAddress}", MedicalRecord.Patient.Address);
                document.ReplaceText("{PatientPhone}", MedicalRecord.Patient.PhoneNumber);
                document.ReplaceText("{PatientEmail}", MedicalRecord.Patient.Email);
                document.ReplaceText("{Symptoms}", MedicalRecord.Symptoms);
                document.ReplaceText("{Diagnosis}", MedicalRecord.Diagnosis);
                document.ReplaceText("{Treatment}", MedicalRecord.Treatment);
                document.ReplaceText("{VisitTime}", brotherCode.ConvertDateTime(MedicalRecord.VisitTime));

                // Save the document to a MemoryStream
                using (var stream = new MemoryStream())
                {
                    document.SaveAs(stream);
                    stream.Position = 0; // Reset the stream position to the beginning

                    // Return the file
                    var fileName = "MedicalRecord.docx";
                    var contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    return File(stream.ToArray(), contentType, fileName);
                }
            }
        }

    }
}