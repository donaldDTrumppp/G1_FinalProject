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

namespace Clinic_Management.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly G1_PRJ_DBContext _context;

        public IndexModel(G1_PRJ_DBContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int RoleId { get; set; } = 0;

        [BindProperty(SupportsGet = true)]
        public int StatusId { get; set; } = 0;

        public IList<User> Users { get; set; } = default!;
        public List<Role> Roles { get;set; }
        public List<UserStatus> UserStatuses { get;set; }

        [BindProperty(SupportsGet = true)]
        public string SortField { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; }

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 5;

        public int TotalRecords { get; set; }

        public string Message { get; set; } = "";

        public async Task OnGetAsync(int PageIndex, int RoleId, int StatusId, string SortField, string SortOrder, string Message)
        {
            this.Message = Message;
            int v = (PageIndex != 0 ? this.PageIndex = PageIndex : this.PageIndex = 1);
            v = (RoleId != 0 ? this.RoleId = RoleId : this.RoleId = 0);
            v = (StatusId != 0 ? this.StatusId = StatusId : this.StatusId = 0);
            this.SortField = SortField;
            this.SortOrder = SortOrder;

            // Fetch the Users query first
            var usersQuery = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Status)
                .Include(u => u.Patient)
                .Include(u => u.Staff)
                .ToListAsync();

            // Fetch the Roles and UserStatuses asynchronously
            var userStatusesTask = _context.UserStatuses.ToListAsync();

            // Await the tasks to complete
            var users = await usersQuery;
            Roles = _context.Roles.ToList();
            UserStatuses = _context.UserStatuses.ToList();
            //TotalRecords = await query.CountAsync();
            //Users = await query.Skip((this.PageIndex - 1) * PageSize).Take(PageSize).ToListAsync();
        }
    }
}
