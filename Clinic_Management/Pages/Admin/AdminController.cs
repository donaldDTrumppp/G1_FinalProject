using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Clinic_Management.Models;
using Microsoft.AspNetCore.SignalR;

namespace Clinic_Management.Pages.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly G1_PRJ_DBContext _context;

        private readonly IHubContext<SignalrServer> _signalRHub;

        private readonly SignalrServer _signalr;

        public AdminController(G1_PRJ_DBContext context,IHubContext<SignalrServer> signalRHub, SignalrServer signalR)
        {
            _context = context;
            _signalRHub = signalRHub;
            _signalr = signalR;
        }

        // GET: api/Admin
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            return await _context.Users.ToListAsync();
        }

        // GET: api/Admin/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Admin/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPut("status/{id}")]
        public async Task<IActionResult> ChangeUserStatus(int id)
        {

            try
            {
                //_context.Users.Where(u => u.UserId == id);  
                //await _context.SaveChangesAsync();
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                //var status = await _context.UserStatuses.SingleOrDefaultAsync(s => s.StatusName == statusName);

                if (user.StatusId == 3)
                {
                    return NotFound();
                }
                else
                {
                    user.StatusId = 3 - user.StatusId;
                }
                await _context.SaveChangesAsync();
                await CheckAndLogDeactiveUsersAsync();
                return Ok(user.StatusId);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        public async Task CheckAndLogDeactiveUsersAsync()
        {
            // Fetch users with the "Deactive" role
            var deactiveUsers = await _context.Users
                .Where(u => u.StatusId == 2)
                .ToListAsync();

            // Log each deactive user using SignalR
            for (int i = 0; i < deactiveUsers.Count; i++)
            {
                // Log to console
                Console.WriteLine($"User {deactiveUsers[i].Username.ToString()} is deactivated.");

                // Notify clients via SignalR
                await _signalRHub.Clients.Group(deactiveUsers[i].Username.ToString()).SendAsync("ReceiveUserStatusChange", deactiveUsers[i].Username, false);
            }
        }
        // POST: api/Admin
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'G1_PRJ_DBContext.Users'  is null.");
            }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.UserId }, user);
        }

        // DELETE: api/Admin/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
