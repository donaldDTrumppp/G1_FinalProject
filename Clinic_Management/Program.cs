using Clinic_Management.Models;
using Clinic_Management.Models;
using Clinic_Management.Services;
using Clinic_Management.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Text;

namespace Clinic_Management
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddDbContext<G1_PRJ_DBContext>();
            builder.Services.AddTransient<EmailService>();

            var configuration = builder.Configuration;

            builder.Services.AddDbContext<G1_PRJ_DBContext>(option =>
            option.UseSqlServer(configuration.GetConnectionString("MyCnn")));
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var jwtSettings = configuration.GetSection("JwtOptions");
            var key = Encoding.UTF8.GetBytes(jwtSettings["SigningKey"]);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var cookie = context.Request.Cookies["AuthToken"];
                        if (!string.IsNullOrEmpty(cookie))
                        {
                            context.Token = cookie;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("StaffPolicy", policy => policy.RequireRole("Doctor", "Receptionist"));
                options.AddPolicy("PatientPolicy", policy => policy.RequireRole("Patient"));
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.UseMiddleware<JwtMiddleware>();

            app.MapRazorPages();
            app.MapGet("/", context =>
            {
                context.Response.Redirect("/Index");
                return Task.CompletedTask;
            });

            app.Run();

        }
    }
}