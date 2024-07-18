//using Clinic_Management.Models;
using Clinic_Management.Models;
using Clinic_Management.Services;
using System.Threading;

namespace Clinic_Management
{
    public class BackgroundWorkerService : BackgroundService
    {
        readonly ILogger<BackgroundWorkerService> _logger;

        readonly G1_PRJ_DBContext _context;

        readonly NotificationService _notificationService;

        readonly EmailService _emailService;

        private readonly IConfiguration _config;

        private readonly IServiceProvider _ServiceProvider;

        /*
        public BackgroundWorkerService(ILogger<BackgroundWorkerService> logger, G1_PRJ_DBContext context, NotificationService notificationService, EmailService emailService, IConfiguration config) { 
            _logger = logger;
            _notificationService = notificationService;
            _context = context; 
            _emailService = emailService;
            _config = config;
        }
        */

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
             //   CheckForIncomingAppointments();
                await Task.Delay(1000 * 60 * 5, stoppingToken);
            }
        }
        
        /*
        private void CheckForIncomingAppointments()
        {
            List<Appointment> appointments = _context.Appointments
                    .Where(a => a.RequestedTime <= DateTime.Now.AddHours(1))
                    .ToList();
            foreach (Appointment appointment in appointments)
            {
                if (appointment != null && appointment.PatientId != null)
                {
                    string activeLink = _config["Host"] + _config["Port"] + "/PatientAppointment/Details?id=" + appointment.PatientId;
                    _notificationService.SendAppointmentNotification((int)appointment.PatientId, "Just about " + DateTime.Now.AddHours(1).Subtract(appointment.RequestedTime).TotalMinutes + " minutes is your appointment", activeLink);
                    var htmlContent = await _emailService.GetAppointmentReminderEmail("appointment_reminder.html", activeLink, appointment.PatientName);
                    _emailService.sendE(Appointment.PatientEmail, "[Appointment] Appointment Reminder", htmlContent);

                }
            }
        }
        */
    }
}