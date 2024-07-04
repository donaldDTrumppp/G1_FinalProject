using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Clinic_Management.Models;

namespace Clinic_Management.Pages.MedicalRecords
{
    public class DetailsModel : PageModel
    {
        private readonly Clinic_Management.Models.G1_PRJ_DBContext _context;

        public DetailsModel(Clinic_Management.Models.G1_PRJ_DBContext context)
        {
            _context = context;
        }

      public MedicalRecord MedicalRecord { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.MedicalRecords == null)
            {
                return RedirectToPage("/Home/404");
            }

            var medicalrecord = await _context.MedicalRecords.FirstOrDefaultAsync(m => m.MedicalrecordId == id);
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
    }
}
