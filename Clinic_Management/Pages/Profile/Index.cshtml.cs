using Clinic_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Management.Pages.Profile
{
    public class IndexModel : PageModel
    {
        private readonly G1_PRJ_DBContext _context;
        public IndexModel(G1_PRJ_DBContext context)
        {
            _context = context;
        }
        public User UserProfile { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            
           

            var username = Request.Cookies["Username"];
            if (username == null)
            {
                return NotFound();
            }

            UserProfile = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (UserProfile == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
