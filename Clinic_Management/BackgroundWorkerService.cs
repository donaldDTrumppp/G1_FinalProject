//using Clinic_Management.Models;
using Clinic_Management.Models;
using Clinic_Management.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Threading;

namespace Clinic_Management
{
    public class BackgroundWorkerService : BackgroundService
    {
        readonly ILogger<BackgroundWorkerService> _logger;




        private readonly IServiceScopeFactory _scopeFactory;


        public BackgroundWorkerService(IServiceScopeFactory scopeFactory, ILogger<BackgroundWorkerService> logger)
        {
            _logger = logger;

            _scopeFactory = scopeFactory;
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ServiceStarted");

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ServiceStopped");
            return Task.CompletedTask;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            while (true)
            {
                
                using (var scope = _scopeFactory.CreateScope())
                {
                    
                    var context = scope.ServiceProvider.GetRequiredService<G1_PRJ_DBContext>();
                    var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();
                    var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();
                    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                    UpdateAppointmentNotComingStatus(context, notificationService, emailService, config);
                    CheckForIncomingAppointments(context, notificationService, emailService, config);
                    await Task.Delay(1000, stoppingToken);
                }
                
                await Task.Delay(1000, stoppingToken);
            }
        }

        
        private async Task UpdateAppointmentNotComingStatus(G1_PRJ_DBContext context, NotificationService notificationService, EmailService emailService, IConfiguration config)
        {
            try
            {

                List<Appointment> appointments = context.Appointments
                    .Where(a => a.StatusNavigation.StatusName == "Scheduled" || a.StatusNavigation.StatusName == "Rescheduled")
                    .Where(a => a.MedicalRecord == null)
                    .Where(a => EF.Functions.DateDiffDay(a.RequestedTime, DateTime.Now) >= 1)
                    .Include(a => a.Patient)
                    .Include(a => a.MedicalRecord)
                    .ToList();

                for (int i = 0; i < appointments.Count; i++)
                {
                    appointments[i].Status = context.AppointmentStatuses.FirstOrDefault(s => s.StatusName == "Not coming").StatusId;
                    context.Appointments.Update(appointments[i]);
                    context.SaveChanges();
                    if (appointments[i].PatientId != null)
                    {
                        notificationService.SendAppointmentNotification((int)appointments[i].PatientId, "You missed your appointment on " + appointments[i].RequestedTime, "");
                    }
                    
                    var htmlContent = await emailService.GetAppointmentNotComingEmail("appointment_not_coming.html", appointments[i].RequestedTime, appointments[i].PatientName);
                    emailService.SendEmailNoHeader(appointments[i].PatientEmail, "[Appointment] You Missed Your Appointment", htmlContent);
                    
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }


        private async Task CheckForIncomingAppointments(G1_PRJ_DBContext context, NotificationService notificationService, EmailService emailService, IConfiguration config)
        {

            using (var scope = _scopeFactory.CreateScope())
            {

                List<Appointment> appointments = context.Appointments
                    .Where(a => a.StatusNavigation.StatusName == "Scheduled" || a.StatusNavigation.StatusName == "Rescheduled")
                    .Where(a => a.Reminded == null || a.Reminded == false)
                    .Where(a => a.RequestedTime <= DateTime.Now.AddHours(1) && a.RequestedTime >= DateTime.Now)
                    .Include(a => a.Patient)
                    .ToList();

                foreach (Appointment appointment in appointments)
                {
                    if (appointment != null)
                    {
                        appointment.Reminded = true;
                        context.Update(appointment);
                        context.SaveChanges();
                        string activeLink = config["Host"] + config["Port"] + "/PatientAppointment/Details?id=" + appointment.AppointmentId;
                        if (appointment.PatientId != null)
                        {
                            notificationService.SendAppointmentNotification((int)appointment.PatientId, "Just about " + Math.Ceiling(appointment.RequestedTime.Subtract(DateTime.Now).TotalMinutes) + " minutes is your appointment", activeLink);
                        }
                        var htmlContent = await emailService.GetAppointmentReminderEmail("appointment_reminder.html", activeLink, appointment.PatientName);
                        emailService.SendEmailNoHeader(appointment.PatientEmail, "[Appointment] Appointment Reminder", htmlContent);

                    }
                }
            }

        }
        

    }
}