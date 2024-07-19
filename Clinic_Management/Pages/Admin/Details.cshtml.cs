using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Clinic_Management.Models;

namespace Clinic_Management.Pages.Admin
{
    public class DetailsModel : PageModel
    {
        private readonly G1_PRJ_DBContext _context;

        public DetailsModel(G1_PRJ_DBContext context)
        {
            _context = context;
        }

        public List<Role> roles;

        public List<Branch> Branchs { get; set; }

        public List<Specialist> Specialists { get; set; }

        public User User { get; set; } = default!;

        [BindProperty]
        public Patient Patient { get; set; }

        [BindProperty]
        public Staff Staff { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Specialists = _context.Specialists.ToList();
            Branchs = _context.Branches.ToList();
            roles = _context.Roles.ToList();
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }
            else 
            {
                User = user;
                if(User.Role.RoleName == "Patient")
                {
                    Patient = _context.Patients.FirstOrDefault(s => s.PatientId == User.UserId);
                }
                else
                {
                    Staff = _context.Staff.FirstOrDefault(s => s.UserId == User.UserId);

                }
            }
            return Page();
        }
    }
}
