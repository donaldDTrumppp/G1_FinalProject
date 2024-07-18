using Microsoft.AspNetCore.Mvc;

namespace Clinic_Management.Pages.Authentication
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetToken()
        {
            //var token = HttpContext.Session.GetString("JWToken");
            var token = Request.Cookies["AuthToken"];
            return Ok(new { Token = token });
        }
    }
}
