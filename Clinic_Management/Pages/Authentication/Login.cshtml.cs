using System.Linq;
using System.Threading.Tasks;
using Clinic_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Management.Pages.Authentication
{
    public class LoginModel : PageModel
    {
        private readonly G1_PRJ_DBContext _context;

        public LoginModel(G1_PRJ_DBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ErrorMessage { get; set; }

        public class InputModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _context.Users
                .Where(u => u.Username == Input.Username && u.Password == Input.Password)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                ErrorMessage = "Invalid login attempt.";
                return Page();
            }


            return RedirectToPage("/Home/Home");
        }
    }
}
