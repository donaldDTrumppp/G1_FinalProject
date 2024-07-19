using Clinic_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;

namespace Clinic_Management.Pages.Authentication
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly G1_PRJ_DBContext _context;

        public ConfirmEmailModel(G1_PRJ_DBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid token");
            }

            var (isValid, userId, email) = TokenMail.ValidateToken(token);
            if (!isValid)
            {
                return BadRequest("Invalid or expired token");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.Email != email)
            {
                return NotFound("User not found");
            }

            if (user.StatusId == 3)
            {
                user.StatusId = 1;
                await _context.SaveChangesAsync();
                TempData["ConfirmationMessage"] = "Your email has been confirmed successfully. You can now log in to your account.";
            }
            else
            {
                TempData["ConfirmationMessage"] = "Your email was already confirmed or your account status is invalid.";
            }

            return Page();
        }
    }
}

