using Clinic_Management.Models;
using System.IdentityModel.Tokens.Jwt;

namespace Clinic_Management.Services
{
    public class UserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly G1_PRJ_DBContext _context;

        public UserContextService(IHttpContextAccessor httpContextAccessor, G1_PRJ_DBContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public User GetUserFromContext()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "userId");
            return _context.Users.FirstOrDefault(u => u.UserId.ToString() == userIdClaim.Value);
        }
    }
}
