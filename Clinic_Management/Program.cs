using Clinic_Management.Models;
using Clinic_Management.Utilities.MailSender;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Clinic_Management
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorPages();
            builder.Services.AddTransient<IMailSender, MailSender>();

            var configuration = builder.Configuration;

            builder.Services.AddDbContext<G1_PRJ_DBContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("MyCnn")));
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
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("StaffPolicy", policy => policy.RequireRole("Bác sĩ", "Lễ tân"));
                options.AddPolicy("PatientPolicy", policy => policy.RequireRole("Bệnh nhân"));
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
