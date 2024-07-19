using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Clinic_Management.Models;
using DocumentFormat.OpenXml.ExtendedProperties;
using Clinic_Management.Utils;
using Microsoft.AspNetCore.Authorization;

namespace Clinic_Management.Pages.Admin
{
    [Authorize(Policy = "AdminPolicy")]
    public class CreateModel : PageModel
    {
        private readonly G1_PRJ_DBContext _context;

        private readonly PasswordService _passwordService;

        public List<Role> roles;
        public CreateModel(G1_PRJ_DBContext context, PasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public IActionResult OnGet()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId");
            ViewData["StatusId"] = new SelectList(_context.UserStatuses, "StatusId", "StatusId");
            Specialists = _context.Specialists.ToList();
            Branchs = _context.Branches.ToList();
            roles = _context.Roles.ToList();
            return Page();
        }

        [BindProperty]
        public User Users { get; set; } = default!;

        [BindProperty(SupportsGet =true)]
        public string HealthInsurance { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime HiredDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public string NationalId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int BranchId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int SpecialistId { get; set; }

        public List<Branch> Branchs { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Gender { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ErrorMessage { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string ErrorType { get; set; }


        public List<Specialist> Specialists { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            Specialists = _context.Specialists.ToList();
            Branchs = _context.Branches.ToList();
            roles = _context.Roles.ToList();
            Users.Password = _passwordService.HashPassword(_passwordService.GenerateRandomPassword(8));
            Users.Role = _context.Roles.FirstOrDefault(r => r.RoleId == Users.RoleId);
            if ( _context.Users == null || User == null)
            {

                return Page();
            }
            if(DetectFault(Users.PhoneNumber, Users.Email, Users.Username, "") == true)
            {
                return Page();
            }
            _context.Users.Add(Users);
            _context.SaveChanges();
            if (Users.Role.RoleName == "Patient")
            {
                Patient p = new Patient();
                p.PatientId = Users.UserId;
                _context.Patients.Add(p);
            }
            else if (Users.Role.RoleName == "Receptionist")
            {
                Staff s = new Staff();
                s.UserId = Users.UserId;
                s.HireDate = HiredDate;
                s.Cccd = NationalId;
                s.Image = "...";
                if (DetectFault(Users.PhoneNumber, Users.Email, Users.Username, s.Cccd) == true)
                {
                    return Page();
                }
                s.DoctorDepartmentId = BranchId;
                _context.Staff.Add(s);
            }
            else if (Users.Role.RoleName == "Doctor")
            {
                Staff s = new Staff();
                s.UserId = Users.UserId;
                s.HireDate = HiredDate;
                s.Cccd = NationalId;
                s.Image = "...";
                if (DetectFault(Users.PhoneNumber, Users.Email, Users.Username, s.Cccd) == true)
                {
                    return Page();
                }
                s.DoctorDepartmentId = BranchId;
                s.DoctorSpecialist = SpecialistId;
                _context.Staff.Add(s);
            }
            else if (Users.Role.RoleName == "Admin")
            {
                Staff s = new Staff();
                s.UserId = Users.UserId;
                s.HireDate = HiredDate;
                s.Image = "...";
                s.Cccd = NationalId;
                if (DetectFault(Users.PhoneNumber, Users.Email, Users.Username, s.Cccd) == true)
                {
                    return Page();
                }
                s.DoctorDepartmentId = BranchId;
                _context.Staff.Add(s);
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index", new { Message = "Create user successfully"});
        }

        public bool DetectFault(string phone, string email, string username, string cccd)
        {
            if(_context.Users.FirstOrDefault(u => u.PhoneNumber == phone) != null)
            {
                ErrorMessage = "Phone is existed";
                ErrorType = "Phone";
                return true;
            } 
            else if (_context.Users.FirstOrDefault(u => u.Email == email) != null)
            {
                ErrorMessage = "Email is existed";
                ErrorType = "Email";
                return true;
            }
            else if (_context.Users.FirstOrDefault(u => u.Username == username) != null)
            {
                ErrorMessage = "Username is existed";
                ErrorType = "Username";
                return true;
            }
            else if (!string.IsNullOrEmpty(cccd) && _context.Users.FirstOrDefault(u => u.Staff.Cccd == cccd) != null)
            {
                ErrorMessage = "National id is existed";
                ErrorType = "NationalId";
                return true;
            }
            return false;
        }
        
    }
}
