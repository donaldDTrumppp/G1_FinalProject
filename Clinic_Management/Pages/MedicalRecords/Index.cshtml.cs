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
    public class IndexModel : PageModel
    {
        private readonly Clinic_Management.Models.G1_PRJ_DBContext _context;

        public IndexModel(Clinic_Management.Models.G1_PRJ_DBContext context)
        {
            _context = context;
        }

        public IList<MedicalRecord> MedicalRecord { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.MedicalRecords != null)
            {
                MedicalRecord = await _context.MedicalRecords
                .Include(m => m.Appointment)
                .Include(m => m.Doctor)
                .Include(m => m.Patient).ToListAsync();
            }
        }
    }
}
