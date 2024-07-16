using Clinic_Management.Models;
using Clinic_Management.Utilities.MailSender;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Clinic_Management.Pages.Authentication
{
    public class RegisterModel : PageModel
    {
        private readonly G1_PRJ_DBContext _context;
        private readonly IMailSender _mailSender;

        public RegisterModel(G1_PRJ_DBContext context, IMailSender mailSender)
        {
            _context = context;
            _mailSender = mailSender;
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
        public int RoleId { get; set; } = 4;
        [BindProperty]
        public int StatusId { get; set; } = 1;



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
           

            var user = new User();
            user.Name = Name;
            user.Dob = Dob;
            user.PhoneNumber = PhoneNumber;
            user.Email = Email;
            user.Address = Address;
            user.RoleId = RoleId;
            user.Username = Username;
            user.Password = HashPassword(Password);
            user.StatusId = StatusId;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate and send confirmation email
            //var token = Guid.NewGuid().ToString();
            //HttpContext.Session.SetString("Confirmation_Email", Email);
            //HttpContext.Session.SetString("Confirmation_Token", token);

            //var confirmationLink = Url.Page(
            //    "/Authentication/ConfirmEmail",
            //    pageHandler: null,
            //    values: new { token },
            //    protocol: Request.Scheme);

            //await _mailSender.SendMailAsync(user.Email, "Confirm your email",
            //    $"Please confirm your email by <a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>clicking here</a>.");

            return RedirectToPage("/Index");
        }


        private string HashPassword(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            string hashedPassword = Convert.ToBase64String(hashBytes);
            return hashedPassword;
        }

    }
}
