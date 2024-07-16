using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Clinic_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Clinic_Management.Pages.Authentication
{
    public class LoginModel : PageModel
    {
        private readonly G1_PRJ_DBContext _context;
        private readonly IConfiguration _configuration;

        public LoginModel(G1_PRJ_DBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [BindProperty]
        [Required(ErrorMessage = "Please enter your username or email.")] 
        public string Username { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please enter your password.")]
        public string Password { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
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

            var token = GenerateJwtToken(user, role);

            HttpContext.Session.SetString("JWToken", token);
            HttpContext.Session.SetString("Username", user.Username);



            return Redirect("/Home/Home");
        }


        private string GenerateJwtToken(User user, string roleName)
        {
            var jwtSettings = _configuration.GetSection("JwtOptions");
            var key = Encoding.UTF8.GetBytes(jwtSettings["SigningKey"]);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("userId", user.UserId.ToString()),
                    new Claim("name", user.Name),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                    new Claim("dob", user.Dob.ToString("yyyy-MM-dd")),
                    new Claim("phoneNumber", user.PhoneNumber),
                    new Claim("email", user.Email),
                    new Claim("address", user.Address),
                    new Claim("userrole", user.RoleId.ToString())
                    
                }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        
    }
}
