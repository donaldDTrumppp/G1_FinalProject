using Clinic_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Clinic_Management.Pages.Authentication
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly G1_PRJ_DBContext _context;

        public ConfirmEmailModel(G1_PRJ_DBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Message { get; set; }

        public async Task<IActionResult> OnGetAsync(string token, string Message)
        {
            this.Message = Message;
            if (string.IsNullOrEmpty(token))
            {
                Message = "Invalid token.";
                return Page();
            }

            var (isValid, userId, email) = TokenMail.ValidateToken(token);

            if (!isValid)
            {
                Message = "The verification link has expired or is invalid. Please request a new one.";
                return Page();
            }

            var user = await _context.Users.FindAsync(userId);

            if (user == null || user.Email != email)
            {
                Message = "User not found or email mismatch.";
                return Page();
            }

            if (user.StatusId == 1)
            {
                Message = "Your account is already verified.";
                return Page();
            }

            user.StatusId = 1; // Active

            await _context.SaveChangesAsync();

            Message = "Your email has been verified successfully. You can now log in.";
            return RedirectToPage("/Authentication/Login", new {Message = "Verify successfully. You can now login"});
        }
    }
}
