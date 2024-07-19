using Clinic_Management.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Management.Services
{
    public class NotificationService
    {
        private readonly G1_PRJ_DBContext _context;

        private readonly IHubContext<SignalrServer> _signalRHub;

        private readonly SignalrServer _signalr;

        public NotificationService(G1_PRJ_DBContext context, IHubContext<SignalrServer> signalRHub, SignalrServer signalr)
        {
            _context = context;
            _signalRHub = signalRHub;
            _signalr = signalr;
        }

        public async Task SendSystemMessage(int userId, string content, string link)
        {
            Notification n = new Notification();
            n.ReceiverId = userId;
            User user = _context.Users.FirstOrDefault(u => u.UserId == userId);
          
            n.Content = content;
            n.Subject = "System Message";
            n.Link = link;
            n.Datetime = DateTime.Now;
            n.Type = _context.NotificationTypes.FirstOrDefault(nt => nt.TypeName == "System").TypeId;
            if (string.IsNullOrEmpty(link))
            {
                n.IsRead = true;
            }
            else
            {
                n.IsRead = false;
            }
            _context.Notifications.Add(n);
            _context.SaveChanges();
            _signalRHub.Clients.Group(user.Username.ToString()).SendAsync("LoadNotifications", n);
        }

        public async Task SendMedicalRecordNotification(int userId, string content, string link)
        {
            Notification n = new Notification();
            n.ReceiverId = userId;
            User user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            n.Content = content;
            n.Subject = "Medical Record";
            n.Link = link;
            n.Datetime = DateTime.Now;
            n.Type = _context.NotificationTypes.FirstOrDefault(nt => nt.TypeName == "Medical").TypeId;
            if (string.IsNullOrEmpty(link))
            {
                n.IsRead = true;
            }
            else
            {
                n.IsRead = false;
            }
            _context.Notifications.Add(n);
            _context.SaveChanges();
            _signalRHub.Clients.Group(user.Username.ToString()).SendAsync("LoadNotifications", n);
        }

        public async Task SendAppointmentNotification(int userId, string content, string link)
        {
            Notification n = new Notification();
            n.ReceiverId = userId;
            User user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            n.Content = content;
            n.Subject = "Appointment";
            n.Link = link;
            n.Datetime = DateTime.Now;
            n.Type = _context.NotificationTypes.FirstOrDefault(nt => nt.TypeName == "Appointment").TypeId;
            if (string.IsNullOrEmpty(link))
            {
                n.IsRead = true;
            }
            else
            {
                n.IsRead = false;
            }
            Console.WriteLine(user.Username);
            _context.Notifications.Add(n);
            _context.SaveChanges();
            _signalRHub.Clients.Group(user.Username.ToString()).SendAsync("LoadNotifications", n);

        }

        public async Task SendAppointmentNotificationToAllReceptionist(int branchId, string content, string link)
        {
            List<Staff> staffs = _context.Staff
                .Where(s => s.DoctorDepartmentId == branchId && s.User.Role.RoleName == "Receptionist")
                .Include(s => s.User)
                .ToList();
            for (int i = 0; i < staffs.Count; i++)
            {
                Notification n = new Notification();
                n.ReceiverId = staffs[i].UserId;
                n.Content = content;
                n.Subject = "Appointment";
                n.Link = link;
                n.Datetime = DateTime.Now;
                n.Type = _context.NotificationTypes.FirstOrDefault(nt => nt.TypeName == "Appointment").TypeId;
                if (string.IsNullOrEmpty(link))
                {
                    n.IsRead = true;
                }
                else
                {
                    n.IsRead = false;
                }
                _context.Notifications.Add(n);
                _context.SaveChanges();
                _signalRHub.Clients.Group(staffs[i].User.Username.ToString()).SendAsync("LoadNotifications", n);
            }
        }
    }
}
