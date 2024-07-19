using Clinic_Management.Models;
using Clinic_Management.Services;
using Clinic_Management.Utilities.MailSender;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Clinic_Management.Pages.Authentication
{
    public class RegisterModel : PageModel
    {
        private readonly G1_PRJ_DBContext _context;
        private readonly Clinic_Management.Utils.Authentication _authentication;
        private readonly EmailService _emailService;
        private readonly IConfiguration _config;
        private const string OtpCookieName = "OTP";
        private const string OtpExpiryCookieName = "OTPExpiry";
        public RegisterModel(G1_PRJ_DBContext context, Utils.Authentication authentication, EmailService emailService, IConfiguration congif)
        {
            _context = context;
            _authentication = authentication;
            _emailService = emailService;
            _config = congif;
        }

        [BindProperty]
        [Required(ErrorMessage = "Please enter your username.")]
        [StringLength(26, MinimumLength = 6, ErrorMessage = "Username must be from 6 to 26 characters.")]
        public string Username { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please enter your password.")]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 30 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)[A-Za-z\d]+$", ErrorMessage = "Password must contain at least one uppercase letter and one number, and no special characters.")]
        public string Password { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please enter your name.")]
        public string Name { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please enter your date of birth.")]
        [DataType(DataType.Date)]
        public DateTime Dob { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please enter your phone number.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be 10 digits.")]
        public string PhoneNumber { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please enter your email.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please enter your address.")]
        public string Address { get; set; }

        [BindProperty]
        public int RoleId { get; set; } = 1;
        [BindProperty]
        public int StatusId { get; set; } = 3;

        public string Message { get; set; } = "";


        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (_context.Users.Any(u => u.Username == Username))
            {
                ModelState.AddModelError("Username", "Username already exists. Please choose a different username.");
                return Page();
            }

            if (_context.Users.Any(u => u.Email == Email))
            {
                ModelState.AddModelError("Email", "Email already exists. Please use a different email.");
                return Page();
            }

            if (_context.Users.Any(u => u.PhoneNumber == PhoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "Phone number already exists. Please use a different phone number.");
                return Page();
            }

            var user = new User
            {
                Name = Name,
                Dob = Dob,
                PhoneNumber = PhoneNumber,
                Email = Email,
                Address = Address,
                RoleId = RoleId,
                Username = Username,
                Password = _authentication.HashPassword(Password),
                StatusId = StatusId
            };

            _context.Users.Add(user);
           
           
            _context.SaveChangesAsync();
            var token = TokenMail.GenerateToken(user.UserId, user.Email);
            var confirmationLink = Url.Page(
                "/Authentication/ConfirmEmail",
                pageHandler: null,
                values: new { token },
                protocol: Request.Scheme);

            string activeLink = _config["Host"] + _config["Port"] + "/Authentication/ConfirmEmail";
            var htmlContent = await _emailService.GetRegisterEmail("register.html", confirmationLink, user.Name);
            _emailService.SendEmailNoHeader(user.Email, "[Register] Verify Account", htmlContent);

            Message = "Registration successful. Please check your email and verify your account within 1 minute.";
            return RedirectToPage("/Authentication/Login");

         
        }
    }
}
