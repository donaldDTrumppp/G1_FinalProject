//using Clinic_Management.Models;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Management
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();


            var configuration = builder.Configuration;

            //builder.Services.AddDbContext<G1_PRJ_DBContext>(option =>
            //option.UseSqlServer(configuration.GetConnectionString("MyCnn")));
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllers();
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