using AspNetCoreRateLimit;
using Clinic_Management.Models;
using Clinic_Management.Services;
using Clinic_Management.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Clinic_Management
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(7200, listenOptions => listenOptions.UseHttps()); // HTTPS
                options.ListenAnyIP(5270); // HTTP
                options.ListenAnyIP(8888, listenOptions => listenOptions.UseHttps()); // Additional HTTPS URL
                options.ListenAnyIP(9999, listenOptions => listenOptions.UseHttps()); // Additional HTTPS URL
            });

            // Configure logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            builder.Services.AddMemoryCache();
            builder.Services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
            builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddDbContext<G1_PRJ_DBContext>();
            

            builder.Services.AddTransient<EmailService>();
            builder.Services.AddTransient<SignalrServer>();
            builder.Services.AddTransient<UserContextService>();
            builder.Services.AddTransient<NotificationService>();
            builder.Services.AddTransient<Authentication>();
            builder.Services.AddTransient<PasswordService>();

            builder.Services.AddSignalR();
            builder.Services.AddHostedService<BackgroundWorkerService>();

            

            /*
            builder.Services.AddDbContext<G1_PRJ_DBContext>(option =>
            option.UseSqlServer(configuration.GetConnectionString("MyCnn")));
            */

            //Reverse Proxy
            builder.Services.AddReverseProxy()
                .LoadFromConfig(configuration.GetSection("ReverseProxy"));
            builder.Services.AddHealthChecks();
            
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
                options.AddPolicy("DoctorPolicy", policy => policy.RequireRole("Doctor"));
                options.AddPolicy("StaffPolicy", policy => policy.RequireRole("Doctor", "Receptionist"));
                options.AddPolicy("PatientPolicy", policy => policy.RequireRole("Patient"));
                options.AddPolicy("ReceptionistPolicy", policy => policy.RequireRole("Receptionist"));
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

            // ReverseProxy Map

            app.MapReverseProxy();
            app.MapHealthChecks("health");

            //app.UseEndpoints(endpoints =>
            //{
            //    // ReverseProxy Map
            //    app.MapReverseProxy();
            //    app.MapHealthChecks("health");
            //});

            app.MapGet("/", context =>
            {
                context.Response.Redirect("/Index");
                return Task.CompletedTask;
            });
            
            app.Run();

        }
    }
}