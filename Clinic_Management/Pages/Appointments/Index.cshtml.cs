using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Clinic_Management.Models;
using ClosedXML.Excel;

namespace Clinic_Management.Pages.Appointements
{
    public class IndexModel : PageModel
    {
        private readonly G1_PRJ_DBContext _context;
        private readonly Clinic_Management.Utils.Authentication authentication;
        private readonly IConfiguration _configuration;
        public IndexModel(G1_PRJ_DBContext context, IConfiguration config)
        {
            _context = context;
            _configuration = config;
            authentication = new Clinic_Management.Utils.Authentication(context, config);
        }

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public int SearchBranch { get; set; } = 0;
        [BindProperty(SupportsGet = true)]
        public int SearchStatus { get; set; } = 0;

        [BindProperty(SupportsGet = true)]
        public string SortField { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; }
        public IList<Appointment> AppointmentList { get; set; }
        public IList<Branch> BranchList { get; set; }
        public IList<AppointmentStatus> StatusList { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int totalRecords { get; set; }
        public String Message { get; set; }
        public int roleID { get; set; }
        public async Task OnGetAsync(string action, int id,string Message)
        {
            string token = HttpContext.Request.Cookies["AuthToken"];
            User u = authentication.GetUserFromToken(token);
            roleID = u.RoleId;
            this.Message = Message;
            var query = _context.Appointments.Include(a => a.Doctor).Include(a => a.Patient).Include(a => a.Branch).Include(a => a.SpecialistNavigation).Include(a => a.StatusNavigation).AsQueryable();         

            if (!string.IsNullOrEmpty(SearchString))
            {
                query = query.Where(a =>
                    a.PatientName.ToLower().Contains(SearchString.ToLower()) ||
                    a.Doctor.Name.ToLower().Contains(SearchString.ToLower())
                );
            }
            if (SearchBranch != 0)
            {
                query = query.Where(q => q.BranchId == SearchBranch);
            }
            if (SearchStatus != 0)
            {
                query = query.Where(q => q.Status == SearchStatus);
            }
            switch (SortField)
            {
                case "PatientName":
                    query = SortOrder == "desc" ? query.OrderByDescending(a => a.PatientName) : query.OrderBy(a => a.PatientName);
                    break;
                case "RequestedDate":
                    query = SortOrder == "desc" ? query.OrderByDescending(r => r.RequestedTime) : query.OrderBy(r => r.RequestedTime);
                    break;
                default:
                    query = query.OrderBy(a => a.PatientName); // Default
                    break;
            }

            totalRecords = await query.CountAsync();

            // Apply pagination
            AppointmentList = await query.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToListAsync();
            BranchList = _context.Branches.ToList();
            StatusList = _context.AppointmentStatuses.ToList();
        }
        public IActionResult OnGetExport()
        {
            using (var workbook = new XLWorkbook())
            {
                var query = _context.Appointments.Include(a => a.Doctor).Include(a => a.Patient).Include(a => a.Branch).Include(a => a.SpecialistNavigation).Include(a => a.StatusNavigation).AsQueryable();
                var worksheet = workbook.Worksheets.Add("Appointment");
                var currentRow = 1;

                // Add headers
                worksheet.Cell(currentRow, 1).Value = "Patient Name";
                worksheet.Cell(currentRow, 2).Value = "Branch";
                worksheet.Cell(currentRow, 3).Value = "Requestedtime";
                worksheet.Cell(currentRow, 4).Value = "Doctor";
                worksheet.Cell(currentRow, 4).Value = "Status";

                // Add data rows
                foreach (var item in query)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.PatientName;
                    worksheet.Cell(currentRow, 2).Value = item.Branch.BranchName;
                    worksheet.Cell(currentRow, 3).Value = item.RequestedTime.ToString();
                    if (item.Doctor != null)
                    {
                        worksheet.Cell(currentRow, 4).Value = item.Doctor.Name;
                    }
                    else
                    {
                        worksheet.Cell(currentRow, 4).Value = "N/A";
                    }
                    
                    worksheet.Cell(currentRow, 5).Value = item.StatusNavigation.StatusName;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Appointments.xlsx");
                }
            }
            return RedirectToAction("/Index");
        }

    }
   
}
