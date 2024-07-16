using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Clinic_Management.Models;
using Clinic_Management.Services;

namespace Clinic_Management.Pages.MedicalRecords
{
    public class DetailsModel : PageModel
    {
        private readonly Clinic_Management.Models.G1_PRJ_DBContext _context;

        private readonly IConfiguration _config;

        private readonly EmailService _emailService;

        public DetailsModel(Clinic_Management.Models.G1_PRJ_DBContext context, EmailService emailService, IConfiguration config)
        {
            _context = context;
            _emailService = emailService;
            _config = config;
        }

        public MedicalRecord MedicalRecord { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.MedicalRecords == null)
            {
                return NotFound();
            }

            var medicalrecord = await _context.MedicalRecords
                .Include(m => m.Doctor)
                .Include(m => m.Appointment)
                .Include(m => m.Appointment.Branch)
                .FirstOrDefaultAsync(m => m.MedicalrecordId == id);
            if (medicalrecord == null)
            {
                return NotFound();
            }
            else
            {
                MedicalRecord = medicalrecord;
                string activeLink = _config["Host"] + _config["Port"] + "/MedicalRecords/Details?id=" + MedicalRecord.MedicalrecordId;
                var htmlContent = await _emailService.GetMedicalRecordEmail("medical_record_edited.html", MedicalRecord.Appointment.Branch.BranchName, MedicalRecord.Appointment.PatientName, MedicalRecord.Appointment.PatientAddress, MedicalRecord.Appointment.PatientDob.ToString(), MedicalRecord.Appointment.PatientPhoneNumber, MedicalRecord.Appointment.PatientEmail, MedicalRecord.Symptoms, MedicalRecord.Diagnosis, MedicalRecord.Treatment, activeLink, MedicalRecord.Doctor.Name, MedicalRecord.VisitTime);
                _emailService.SendEmailMedical("tranhaibang665@gmail.com", "[Medical Record] Medical Record Created Successfully", htmlContent);
            }
            return Page();
        }
    }
}
