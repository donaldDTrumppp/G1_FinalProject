using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Clinic_Management.Models;

namespace Clinic_Management.Pages.Appointements
{
    public class IndexModel : PageModel
    {
        private readonly G1_PRJ_DBContext _context;

        public IndexModel(G1_PRJ_DBContext context)
        {
            _context = context;
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
        public int roleID {  get; set; }
        public String Message { get; set; }
        public async Task OnGetAsync(string action, int id,string Message)
        {
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
        
    }
   
}
