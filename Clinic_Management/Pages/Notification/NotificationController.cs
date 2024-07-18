using Clinic_Management.Models;
using Clinic_Management.Services;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Management.Pages.Notification
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {

        private readonly G1_PRJ_DBContext _context;

        private readonly UserContextService _userService;

        public NotificationController(G1_PRJ_DBContext context, UserContextService userService)
        {
            _context = context;
            _userService = userService;
        }

        [HttpGet("{page}")]
        public async Task<ActionResult<List<Clinic_Management.Models.Notification>>> GetNotifications(int page)
        {

            User user = _userService.GetUserFromContext();
            if (user == null)
            {
                return NotFound();
            }
            var query = _context.Notifications
                .Where(n => n.ReceiverId == user.UserId)
                .Include(n => n.TypeNavigation)
                .OrderByDescending(n => n.Datetime)
                .AsQueryable();
            var notifications = await query.Skip((page - 1) * 5).Take(5).ToListAsync();
            return Ok(notifications);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutUpdateNotifications(int id)
        {
            Clinic_Management.Models.Notification notification = _context.Notifications.FirstOrDefault(n => n.NotificationId == id);
            if (notification != null)
            {
                notification.IsRead = true;
                _context.Update(notification);
                _context.SaveChanges();
            }
            return Ok();
        }
    }
}
