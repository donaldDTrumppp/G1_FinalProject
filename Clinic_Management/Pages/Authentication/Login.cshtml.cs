using System.ComponentModel.DataAnnotations;
using Clinic_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Clinic_Management.Utils;

namespace Clinic_Management.Pages.Authentication
{
    public class LoginModel : PageModel
    {
        private readonly G1_PRJ_DBContext _context;
        private readonly IConfiguration _configuration;
        private readonly Utils.Authentication authentication;

        public LoginModel(G1_PRJ_DBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            authentication = new Utils.Authentication(context, configuration);
        }

        [BindProperty]
        [Required(ErrorMessage = "Please enter your username or email.")] 
        public string Username { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please enter your password.")]
        public string Password { get; set; }

        public void OnGet(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (!_context.Users.Any(u => u.Username == Username))
            {
                ModelState.AddModelError("Username", "Username is invalid.");
                return Page();
            }
            if (!_context.Users.Any(u => u.Password == Password))
            {
                ModelState.AddModelError("Password", "Password is invalid.");
                return Page();
            }

            var user = await _context.Users
                .Where(u => (u.Username == Username && u.Password == Password) || (u.Email == Username && u.Password == Password))
                .FirstOrDefaultAsync();


            var role = await _context.Roles
                .Where(r => r.RoleId == user.RoleId)
                .Select(r => r.RoleName)
                .FirstOrDefaultAsync();

            var token = authentication.GenerateJwtToken(user);
            Response.Cookies.Append("AuthToken", token);
            Response.Cookies.Append("Username", user.Name);

            Console.WriteLine(token);
            //return new JsonResult(new { Token = token });
            return Redirect(returnUrl ?? "/Index");
        }
    }
}
