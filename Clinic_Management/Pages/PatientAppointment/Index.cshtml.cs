using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Clinic_Management.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol.Plugins;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Web;
using System.Net.Http.Headers;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using RestSharp;
using Microsoft.Data.SqlClient;
using NuGet.Common;
using Clinic_Management.Services;
using Microsoft.AspNetCore.Html;

namespace Clinic_Management.Pages.PatientAppointment
{
    public class IndexModel : PageModel
    {
        private readonly Clinic_Management.Models.G1_PRJ_DBContext _context;

        private readonly EmailService _emailService;

        private readonly UserContextService _userService;

        private readonly NotificationService _notificationService;

        public IndexModel(Clinic_Management.Models.G1_PRJ_DBContext context, EmailService emailService, UserContextService service, NotificationService notificationService)
        {
            _context = context;
            _emailService = emailService;
            _userService = service;
            _notificationService = notificationService;
        }

        [BindProperty(SupportsGet = true)]
        public int SpecialistId { get; set; } = 0;

        [BindProperty(SupportsGet = true)]
        public int BranchId { get; set; } = 0;

        [BindProperty(SupportsGet = true)]
        public int StatusId { get; set; } = 0;

        public List<Specialist> Specialists { get; set; }

        public List<Branch> Branchs { get; set; }

        public List<AppointmentStatus> Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortField { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; }

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 5;

        public int TotalRecords { get; set; }

        public IList<Appointment> Appointment { get; set; } = default!;

        public string Message { get; set; } = "";

        public async Task OnGetAsync(int PageIndex, int SpecialistId, int BranchId, int StatusId, string SortField, string SortOrder, string Message)
        {
            this.Message = Message;
            int v = (PageIndex != 0 ? this.PageIndex = PageIndex : this.PageIndex = 1);
            v = (SpecialistId != 0 ? this.SpecialistId = SpecialistId : this.SpecialistId = 0);
            v = (BranchId != 0 ? this.BranchId = BranchId : this.BranchId = 0);
            v = (StatusId != 0 ? this.StatusId = StatusId : this.StatusId = 0);
            this.SortField = SortField;
            this.SortOrder = SortOrder;

            var query = _context.Appointments
                .Include(a => a.Branch)
                .Include(a => a.Doctor)
                .Include(a => a.Receptionist)
                .Include(a => a.MedicalRecord)
                .Include(a => a.SpecialistNavigation)
                .Include(a => a.StatusNavigation)
                .AsQueryable();

            if (SpecialistId != 0)
            {
                query = query.Where(a => a.SpecialistNavigation.SpecialistId == SpecialistId);
            }

            if (BranchId != 0)
            {
                query = query.Where(a => a.Branch.BranchId == BranchId);
            }

            if (StatusId != 0)
            {
                query = query.Where(a => a.StatusNavigation.StatusId == StatusId);
            }

            switch (SortField)
            {
                case "RequestedTime":
                    query = SortOrder == "desc" ? query.OrderByDescending(r => r.RequestedTime) : query.OrderBy(r => r.RequestedTime);
                    break;
                case "Receptionist":
                    query = SortOrder == "desc" ? query.OrderByDescending(r => (r.Receptionist != null ? r.Receptionist.Name : "")) : query.OrderBy(r => (r.Receptionist != null ? r.Receptionist.Name : ""));
                    break;
                case "Patient":
                    query = SortOrder == "desc" ? query.OrderByDescending(r => r.PatientName) : query.OrderBy(r => r.PatientName);
                    break;
                case "Doctor":
                    query = SortOrder == "desc" ? query.OrderByDescending(r => r.Doctor != null ? r.Doctor.Name : "") : query.OrderBy(r => r.Doctor != null ? r.Doctor.Name : "ZZZZZZZZZZZZZZZZZZZZZZZZZZZZ");
                    break;
                default:
                    query = query.OrderBy(r => r.CreatedAt); // Default
                    break;
            }
            Specialists = await _context.Specialists.Distinct().ToListAsync();
            Branchs = await _context.Branches.Distinct().ToListAsync();
            Status = await _context.AppointmentStatuses.Distinct().ToListAsync();
            TotalRecords = await query.CountAsync();
            Appointment = await query.Skip((this.PageIndex - 1) * PageSize).Take(PageSize).ToListAsync();
        }

        
    }
}
