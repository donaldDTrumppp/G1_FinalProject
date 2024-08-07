﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Clinic_Management.Utils
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly string[] _excludedPaths = {
            "/Authentication/Login",
            "/Authentication/Register",
            "/Authentication/Logout",
            "/Index",
            "/"
        }; 

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.ToString();

            //Console.WriteLine("current path: " + path);

            // Check if the request path is in the excluded paths
            if (_excludedPaths.Contains(path, StringComparer.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            var token = context.Request.Cookies["AuthToken"];
            if (token == null)
            {
                RedirectToLogin(context);
                return;
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["JwtOptions:SigningKey"]);
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["JwtOptions:Issuer"],
                    ValidAudience = _configuration["JwtOptions:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                // Validate the token
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                // Ensure token is a JWT token
                if (validatedToken is not JwtSecurityToken jwtToken)
                {
                    throw new SecurityTokenException("Invalid token.");
                }

                // Check if token has expired
                if (jwtToken.ValidTo < DateTime.UtcNow)
                {
                    context.Response.Cookies.Delete("AuthToken");
                    RedirectToLogin(context);
                    return;
                }
            }
            catch
            {
                context.Response.Cookies.Delete("AuthToken");
                RedirectToLogin(context);
                return;
            }

            await _next(context);
        }

        private void RedirectToLogin(HttpContext context)
        {
            var returnUrl = context.Request.Path + context.Request.QueryString;

            context.Response.Redirect("/Authentication/Login?returnUrl=" + returnUrl);
        }
    }
}
