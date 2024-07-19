using Clinic_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Threading.Tasks;
using Clinic_Management.Services;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Clinic_Management.Pages.Authentication
{
    public class ResetPasswordModel : PageModel
    {
        private readonly G1_PRJ_DBContext _context;
        private readonly Clinic_Management.Utils.Authentication _authentication;

        public ResetPasswordModel(G1_PRJ_DBContext context, Clinic_Management.Utils.Authentication authentication)
        {
            _context = context;
            _authentication = authentication;
        }

        [BindProperty]
        public string Token { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please enter your username or new pass word.")]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 30 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)[A-Za-z\d]+$", ErrorMessage = "Password must contain at least one uppercase letter and one number, and no special characters.")]
        public string NewPassword { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please enter your username or confirm password.")]
        public string ConfirmPassword { get; set; }

        public string Message { get; set; }

        public bool IsTokenValid { get; set; }

        public IActionResult OnGet(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Index");
            }

            var (isValid, userId, email) = TokenMail.ValidateToken(token);
            if (!isValid)
            {
                Message = "Invalid or expired token. Please request a new password reset.";
                IsTokenValid = false;
                return Page();
            }

            Token = token;
            IsTokenValid = true;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (NewPassword != ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "The new password and confirmation password do not match.");
                return Page();
            }

            var (isValid, userId, email) = TokenMail.ValidateToken(Token);
            if (!isValid)
            {
                Message = "Invalid or expired token. Please request a new password reset.";
                return Page();
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                Message = "User not found.";
                return Page();
            }

            // Hash the new password
            string hashedPassword = _authentication.HashPassword(NewPassword);

            // Update the user's password
            user.Password = hashedPassword;
            await _context.SaveChangesAsync();

            Message = "Your password has been reset successfully. You can now log in with your new password.";
            return Redirect("/Authentication/Login");
        }
    }
}
