using Clinic_Management.Models;
using Clinic_Management.Utils;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Clinic_Management
{
    public class SignalrServer : Hub
    {

        private readonly G1_PRJ_DBContext _context;

        private readonly IHttpContextAccessor _contextAccessor;

        private readonly Authentication _authentication;

        public SignalrServer(G1_PRJ_DBContext context, IHttpContextAccessor contextAccessor, Authentication authentication)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _authentication = authentication;
        }

        public override async Task OnConnectedAsync()
        {
            var token = _contextAccessor.HttpContext.Request.Query["access_token"];
            Console.WriteLine("Connected");
            Console.WriteLine(token);
            if (!string.IsNullOrEmpty(token))
            {
                int userId = _authentication.GetUserIdFromToken(token);
                User user = _context.Users.FirstOrDefault(u => u.UserId == userId);
                if (user != null)
                {
                    Console.WriteLine("Connected: " + user.Username);
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, user.Username);
                    await Groups.AddToGroupAsync(Context.ConnectionId, user.Username);
                }
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var token = _contextAccessor.HttpContext.Request.Query["access_token"];
            if (!string.IsNullOrEmpty(token))
            {
                int userId = _authentication.GetUserIdFromToken(token);
                User user = _context.Users.FirstOrDefault(u => u.UserId == userId);
                if (user != null)
                {
                    Console.WriteLine("Disconnected: " + user.Username);
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, user.Username);
                }
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
