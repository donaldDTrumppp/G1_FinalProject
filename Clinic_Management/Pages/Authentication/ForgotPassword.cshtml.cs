using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Clinic_Management.Services;
using Microsoft.Extensions.Configuration;
using Clinic_Management.Models;

namespace Clinic_Management.Pages.Authentication
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly G1_PRJ_DBContext _context;
        private readonly EmailService _emailService;
        private readonly IConfiguration _config;

        public ForgotPasswordModel(G1_PRJ_DBContext context,EmailService emailService, IConfiguration config)
        {
            _context = context;
            _emailService = emailService;
            _config = config;
        }

        [BindProperty]
        public string Email { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid)
            {
                return Page();
            }
            User user = _context.Users.FirstOrDefault(u => u.Email == Email);

            if (user != null)
            {
                var token = TokenMail.GenerateToken(user.UserId, user.Email);
                var confirmationLink = Url.Page(
                    "/Authentication/ResetPassword",
                    pageHandler: null,
                    values: new { token },
                    protocol: Request.Scheme);

                string activeLink = _config["Host"] + _config["Port"] + "/Authentication/ResetPassword";
                var htmlContent = await _emailService.GetForgotPasswordEmail("forgot_password.html", confirmationLink, user.Name);
                await _emailService.SendEmailNoHeader(user.Email, "[Clinic Management] Reset Password", htmlContent);



                TempData["Message"] = "A reset link has been sent to your email.";
                return RedirectToPage();
            }
            else
            {
                ModelState.AddModelError("Email", "Your account does not exist");
                return Page();
            }

           
        }
    }
}
