using Clinic_Management.Models;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace Clinic_Management.Pages.MedicalRecords
{
    public class IndexModel : PageModel
    {
        private readonly G1_PRJ_DBContext _context;
        public IndexModel(G1_PRJ_DBContext context)
        {
            _context = context;
        }
        public IList<MedicalRecord> MedicalRecord { get; set; }
        public string Message { get; set; } = "";
        public string TypeMessage { get; set; } = "";

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
        public SelectList Branchlist { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? BranchFilter { get; set; }
        #endregion

        #region Paging
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int totalRecords { get; set; } = 1;
        #endregion
        public async Task OnGetAsync(string? TypeMessage, string? Message)
        {
            this.TypeMessage = TypeMessage;
            this.Message = Message;
            // Get specialists for the dropdown list
            var specialistsQuery = from s in _context.Specialists
                                   select s;

            Specialists = new SelectList(await specialistsQuery.ToListAsync(), "SpecialistId", "SpecialistName");

            var branchQuery = from s in _context.Branches
                                   select s;

            Branchlist = new SelectList(await branchQuery.ToListAsync(), "BranchId", "BranchName");


            var query = _context.MedicalRecords.Include(a => a.Appointment).ThenInclude(s => s.SpecialistNavigation).Include(m => m.Doctor).Include(m => m.Patient).AsQueryable();

            if (SpecialistFilter.HasValue)
            {
                query = query.Where(m => m.Appointment.Specialist == SpecialistFilter.Value);
            }

            if (BranchFilter.HasValue)
            {
                query = query.Where(m => m.Appointment.BranchId == BranchFilter.Value);
            }

            if (!string.IsNullOrEmpty(SearchString))
            {
                query = query.Where(m =>
                    //m.Symptoms.ToLower().Contains(SearchString.ToLower()) ||
                    //m.Diagnosis.ToLower().Contains(SearchString.ToLower()) ||
                    //m.Treatment.ToLower().Contains(SearchString.ToLower()) ||
                    m.Doctor.Name.ToLower().Contains(SearchString.ToLower()) ||
                    m.Patient.PatientNavigation.Name.ToLower().Contains(SearchString.ToLower())
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
                    query = SortOrder == "desc" ? query.OrderByDescending(r => r.Patient.PatientNavigation.Name) : query.OrderBy(r => r.Patient.PatientNavigation.Name);
                    break;
            }

            totalRecords = await query.CountAsync();

            // Apply pagination
            //if (PageIndex - 1 >= totalRecords) PageIndex = totalRecords - 1;
            MedicalRecord = await query.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToListAsync();
        }

        [BindProperty]
        public IFormFile File { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (File == null || File.Length == 0)
            {
                ModelState.AddModelError("File", "Please select a file.");
                return Page();
            }

            var fileExtension = Path.GetExtension(File.FileName).ToLower();

            using (var stream = new MemoryStream())
            {
                await File.CopyToAsync(stream);
                stream.Position = 0;

                if (fileExtension == ".json")
                {
                    // Handle JSON file
                    using (var reader = new StreamReader(stream))
                    {
                        var jsonString = await reader.ReadToEndAsync();
                        var medicalRecords = JsonConvert.DeserializeObject<List<MedicalRecord>>(jsonString);
                        // Process medicalRecords
                    }
                }
                else if (fileExtension == ".xml")
                {
                    // Handle XML file
                    var serializer = new XmlSerializer(typeof(List<MedicalRecord>));
                    var medicalRecords = (List<MedicalRecord>)serializer.Deserialize(stream);
                    // Process medicalRecords
                }
                else if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {
                    // Handle Excel file
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheet(1);
                        var medicalRecords = new List<MedicalRecord>();
                        foreach (var row in worksheet.RowsUsed().Skip(1)) // Skip header row
                        {
                            var medicalRecord = new MedicalRecord
                            {
                                MedicalrecordId = row.Cell(1).GetValue<int>(),
                                AppointmentId = row.Cell(2).GetValue<int?>(),
                                PatientId = row.Cell(3).GetValue<int?>(),
                                DoctorId = row.Cell(4).GetValue<int?>(),
                                VisitTime = row.Cell(5).GetValue<DateTime>(),
                                CreatedAt = row.Cell(6).GetValue<DateTime?>(),
                                Symptoms = row.Cell(7).GetValue<string>(),
                                Diagnosis = row.Cell(8).GetValue<string?>(),
                                Treatment = row.Cell(9).GetValue<string?>()
                            };
                            medicalRecords.Add(medicalRecord);
                        }
                        // Process medicalRecords
                    }
                }
                else
                {
                    ModelState.AddModelError("File", "Unsupported file format.");
                    return Page();
                }
            }

            return RedirectToPage("./Index"); // Adjust as needed
        }
    }
}
