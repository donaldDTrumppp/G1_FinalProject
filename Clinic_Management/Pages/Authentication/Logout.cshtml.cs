using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Clinic_Management.Pages.Authentication
{
    public class LogoutModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogoutModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            HttpContext.Response.Cookies.Delete("AuthToken");
            HttpContext.Response.Cookies.Delete("Username");
            return Redirect("/Index");
        }
    }
}
