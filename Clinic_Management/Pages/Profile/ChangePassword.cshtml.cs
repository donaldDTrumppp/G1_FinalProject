using Clinic_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Management.Pages.Profile
{
    public class ChangePasswordModel : PageModel
    {
        private readonly G1_PRJ_DBContext _context;
        private readonly Clinic_Management.Utils.Authentication _authentication;

        public ChangePasswordModel(G1_PRJ_DBContext context, Clinic_Management.Utils.Authentication authentication)
        {
            _context = context;
            _authentication = authentication;
        }

        [BindProperty]
        [Required(ErrorMessage = "Current password is required.")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "New password is required.")]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 30 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)[A-Za-z\d]+$", ErrorMessage = "Password must contain at least one uppercase letter and one number, and no special characters.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Confirm password is required.")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [BindProperty]
        public User UserProfile { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Request.Cookies["AuthToken"];
            var userId = _authentication.GetUserIdFromToken(token);

            if (userId == null)
            {
                return NotFound("User not authenticated.");
            }

            User user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            UserProfile = user;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var token = HttpContext.Request.Cookies["AuthToken"];
            var userId = _authentication.GetUserIdFromToken(token);

            if (userId == null)
            {
                return Redirect("/Authentication/Login");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            if (user.Password != CurrentPassword)
            {
                ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                return Page();
            }

            if (NewPassword != ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "The new password and confirmation password do not match.");
                return Page();
            }

            user.Password = NewPassword;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException($"Unexpected error occurred updating password for user with ID '{userId}'.");
            }

            return Redirect("/Profile/Index");
        }
    }
}
