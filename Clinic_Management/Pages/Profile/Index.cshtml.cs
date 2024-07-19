using Clinic_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Management.Pages.Profile
{
    public class IndexModel : PageModel
    {
        private readonly G1_PRJ_DBContext _context;
        private readonly Clinic_Management.Utils.Authentication _authentication;

        public IndexModel(G1_PRJ_DBContext context, Clinic_Management.Utils.Authentication authentication)
        {
            _context = context;
            _authentication = authentication;
        }

        [BindProperty]
        public User UserProfile { get; set; } = default!;

        [BindProperty]
        public Patient PatientProfile { get; set; } = default!;

        public bool Mode { get; set; } = false;

        public async Task<IActionResult> OnGetAsync(string? Mde)
        {
            var token = HttpContext.Request.Cookies["AuthToken"];
            var userId = _authentication.GetUserIdFromToken(token);
            //User user = _context.Users.
            UserProfile = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == userId);
            PatientProfile = await _context.Patients.Include(p => p.PatientNavigation).FirstOrDefaultAsync(p => p.PatientId == userId);

            if (Mde != null)
            {
                Mode = Mde.ToLower() == "true";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            var userInDb = await _context.Users.FindAsync(UserProfile.UserId);
            var patientInDb = await _context.Patients.FindAsync(PatientProfile.PatientId);

            if (userInDb == null || patientInDb == null)
            {
                return NotFound();
            }

            userInDb.Username = UserProfile.Username;
            userInDb.Name = UserProfile.Name;
            userInDb.Dob = UserProfile.Dob;
            userInDb.PhoneNumber = UserProfile.PhoneNumber;
            userInDb.Email = UserProfile.Email;
            userInDb.Address = UserProfile.Address;

            patientInDb.HealthInsurance = PatientProfile.HealthInsurance;

            try
            {
                _context.Users.Update(userInDb);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(UserProfile.UserId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index", new { Message = "Profile updated successfully" });
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
