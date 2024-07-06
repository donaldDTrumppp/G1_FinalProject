using Clinic_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Management.Pages.MedicalRecords
{
    public class IndexModel : PageModel
    {
        private readonly Clinic_Management.Models.G1_PRJ_DBContext _context;

        public IndexModel(Clinic_Management.Models.G1_PRJ_DBContext context)
        {
            _context = context;
        }
        public IList<MedicalRecord> MedicalRecord { get; set; }

        #region Search
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }
        #endregion

        #region Sort
        [BindProperty(SupportsGet = true)]
        public string SortField { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; }
        #endregion

        #region Filter
        public SelectList Specialists { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? SpecialistFilter { get; set; }
        #endregion

        #region Paging
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int totalRecords { get; set; }
        #endregion
        public async Task OnGetAsync()
        {
            // Get specialists for the dropdown list
            var specialistsQuery = from s in _context.Specialists
                                   orderby s.SpecialistName
                                   select s;

            Specialists = new SelectList(await specialistsQuery.ToListAsync(), "SpecialistId", "SpecialistName");


            var query = _context.MedicalRecords.Include(m => m.Doctor).Include(m => m.Patient).AsQueryable();

            if (SpecialistFilter.HasValue)
            {
                query = query.Where(m => m.Appointment.Specialist == SpecialistFilter.Value);
            }

            if (!string.IsNullOrEmpty(SearchString))
            {
                query = query.Where(m =>
                    //m.Symptoms.ToLower().Contains(SearchString.ToLower()) ||
                    //m.Diagnosis.ToLower().Contains(SearchString.ToLower()) ||
                    //m.Treatment.ToLower().Contains(SearchString.ToLower()) ||
                    m.Doctor.Name.ToLower().Contains(SearchString.ToLower()) ||
                    m.Patient.Name.ToLower().Contains(SearchString.ToLower())
                );
            }

            switch (SortField)
            {
                case "VisitTime":
                    query = SortOrder == "desc" ? query.OrderByDescending(r => r.VisitTime) : query.OrderBy(r => r.VisitTime);
                    break;
                case "Doctor":
                    query = SortOrder == "desc" ? query.OrderByDescending(r => r.Doctor.Name) : query.OrderBy(r => r.Doctor.Name);
                    break;
                case "Patient":
                    query = SortOrder == "desc" ? query.OrderByDescending(r => r.Patient.Name) : query.OrderBy(r => r.Patient.Name);
                    break;
                default:
                    query = query.OrderBy(r => r.VisitTime); // Default
                    break;
            }

            totalRecords = await query.CountAsync();

            // Apply pagination
            MedicalRecord = await query.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToListAsync();
        }
    }
}
