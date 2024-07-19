//using Clinic_Management.Models;
using Clinic_Management.Models;
using Clinic_Management.Services;
using Clinic_Management.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;

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
            builder.Services.AddTransient<SignalrServer>();
            builder.Services.AddTransient<UserContextService>();
            builder.Services.AddTransient<NotificationService>();
            builder.Services.AddTransient<Authentication>();
            builder.Services.AddTransient<PasswordService>();
            /*
            builder.Services.AddSingleton<EmailService>();
            builder.Services.AddSingleton<SignalrServer>();
            builder.Services.AddSingleton<UserContextService>();
            builder.Services.AddSingleton<NotificationService>();
            builder.Services.AddSingleton<Authentication>();
            builder.Services.AddSingleton<PasswordService>();
            */

            builder.Services.AddSignalR();
            builder.Services.AddHostedService<BackgroundWorkerService>();

            var configuration = builder.Configuration;

            /*
            builder.Services.AddDbContext<G1_PRJ_DBContext>(option =>
            option.UseSqlServer(configuration.GetConnectionString("MyCnn")));
            */
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
                        var accessToken = context.Request.Query["access_token"];
                        // If the request is for our SignalR hub...
                        var path = context.HttpContext.Request.Path;
                        var cookie = context.Request.Cookies["AuthToken"];
                        /*
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/signalrServer")))
                        {
                            context.Token = cookie;
                        }
                        */
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
                options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
            });



            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.UseStatusCodePagesWithReExecute("/Errors/{0}");


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.UseMiddleware<JwtMiddleware>();

            app.MapRazorPages();
            app.MapHub<SignalrServer>("/signalrServer");

            app.MapGet("/", context =>
            {
                context.Response.Redirect("/Index");
                return Task.CompletedTask;
            });

            app.Run();

        }
    }
}