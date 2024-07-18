using Clinic_Management.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Clinic_Management.Utils
{
    public class Authentication
    {
        private readonly G1_PRJ_DBContext _context;
        private readonly IConfiguration _configuration;

        public Authentication(G1_PRJ_DBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public string GenerateJwtToken(User user, string roleName)
        {
            var jwtSettings = _configuration.GetSection("JwtOptions");
            var key = Encoding.UTF8.GetBytes(jwtSettings["SigningKey"]);

            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Name, user.Name),
                new Claim("roleId", user.RoleId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtOptions:Issuer"],
                audience: _configuration["JwtOptions:Audience"],
                claims: claims,
                expires: DateTime.Now.AddSeconds(int.Parse(_configuration["JwtOptions:ExpirationSeconds"])),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            );

            //var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public int GetUserIdFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtOptions:SigningKey"]);
            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["JwtOptions:Issuer"],
                    ValidAudience = _configuration["JwtOptions:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.NameId).Value);

                return userId;
            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex.ToString());
                return -1;
            }
        }

        public string GetUsernameFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtOptions:SigningKey"]);
            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["JwtOptions:Issuer"],
                    ValidAudience = _configuration["JwtOptions:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var username = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;

                return username;
            }
            catch
            {
                return null;
            }
        }

        public User GetUserFromToken(string token, string signingKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(signingKey);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            int userId = int.Parse(jwtToken.Claims.First(x => x.Type == "userId").Value);

            return _context.Users.FirstOrDefault(u => u.UserId == userId);
        }

    }
}
