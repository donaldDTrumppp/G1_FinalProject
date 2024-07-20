using Clinic_Management.Models;
using Clinic_Management.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Clinic_Management.Pages.Appointments
{
    [Authorize(Policy = "StaffPolicy")]
    public class CreateModel : PageModel
    {
        private readonly Clinic_Management.Models.G1_PRJ_DBContext _context;
        private readonly Clinic_Management.Utils.Authentication authentication;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly NotificationService _notificationService;
        private readonly IConfiguration _config;

        public CreateModel(Clinic_Management.Models.G1_PRJ_DBContext context,IConfiguration config, EmailService emailService, NotificationService notificationService, IConfiguration configu)
        {
            _context = context;
            _configuration = config;
            _config = configu;
            _emailService = emailService;
            _notificationService = notificationService;
            authentication = new Clinic_Management.Utils.Authentication(context, config);
        }
        public IList<User> patientList { get; set; }
        public IList<Staff> doctorList { get; set; }
        public IList<Specialist> specialistList { get; set; }
        public IList<Branch> branchList { get; set; }

        [BindProperty]
        public int searchPatientID { get; set; }
        [BindProperty]
        public string searchPatientName { get; set; }

        [BindProperty]
        public string fullname { get; set; }

        [BindProperty]
        public string address { get; set; }

        [BindProperty]
        public string dobText { get; set; }

        [BindProperty]
        public string phone { get; set; }

        [BindProperty]
        public string email { get; set; }

        [BindProperty]
        public string health { get; set; }

        [BindProperty]
        public string symptoms { get; set; }

        [BindProperty]
        public int specialistId { get; set; }

        [BindProperty]
        public int branchId { get; set; }

        [BindProperty]
        public int doctorId { get; set; }

        [BindProperty]
        public string requestedDateText { get; set; }


        [BindProperty]
        public int requestedTime { get; set; }

        [BindProperty]
        public string patientError { get; set; }

        [BindProperty]
        public string appointmentError { get; set; }

        public string errorMessage { get; set; }
        public string dateError {  get; set; }
        public string dobError { get; set; }
        public IActionResult OnGet()
        {
            branchList = _context.Branches.ToList();
            specialistList = _context.Specialists.ToList();
            doctorList = _context.Staff.Include(d => d.DoctorDepartment).Include(d => d.DoctorSpecialistNavigation).Include(d => d.User).Where(d => d.User.RoleId == 2).ToList();
            patientList = _context.Users.Include(u=>u.Patient).Where(u => u.RoleId == 1).ToList();
            return Page();
        }
        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            DateTime requestedDate = DateTime.ParseExact(requestedDateText, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime dob = DateTime.ParseExact(dobText, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            branchList = _context.Branches.ToList();
            specialistList = _context.Specialists.ToList();
            doctorList = _context.Staff.Include(d=>d.DoctorDepartment).Include(d=>d.DoctorSpecialistNavigation).Include(d => d.User).Where(d => d.User.RoleId == 2).ToList();
            patientList = _context.Users.Include(u=>u.Patient).Where(u => u.RoleId == 4).ToList();
            
            bool isPatientError = false;
            bool isAppointmentError = false;

            if (DateTime.Parse(requestedDate.ToString("yyyy-MM-dd")) <  DateTime.Parse(DateTime.Now.Date.ToString("yyyy-MM-dd")))
            {
                isAppointmentError = true;
                dateError = "Request date cannot be in the past";
            }
            if( DateTime.Parse(dob.ToString("yyyy-MM-dd")) > DateTime.Parse(DateTime.Now.Date.ToString("yyyy-MM-dd")))
            {
                isPatientError = true;
                dobError = "DOB cannot be in the future";
            }
            var newAppointment = new Appointment();
            if (searchPatientID != 0)
            {
                newAppointment.PatientId = searchPatientID;
            }
            //else
            //{
            //    if (_context.Users.FirstOrDefault(u => u.PhoneNumber.Equals(phone)) != null)
            //    {
            //        patientError += "Phonenumber existed. ";
            //        isPatientError = true;
            //    }
            //    if (_context.Users.FirstOrDefault(u => u.Email.Equals(email)) != null)
            //    {
            //        patientError += "Email existed. ";
            //        isPatientError = true;
            //    }
                
            //}
            newAppointment.PatientName = fullname;
            newAppointment.PatientAddress = address;
            newAppointment.PatientDob = DateTime.Parse(dob.ToString("yyyy-MM-dd"));
            newAppointment.PatientPhoneNumber = phone;
            newAppointment.PatientEmail = email;
            //var patient = _context.Users.FirstOrDefault(p => p.UserId == searchPatientID);
            var doctor = _context.Staff.Include(u => u.User).FirstOrDefault(u => u.UserId == doctorId);
            if (doctor.DoctorDepartmentId != branchId)
            {
                appointmentError += "This doctor is currently working on another branch. ";
                isAppointmentError = true;
            }

            if (doctor.DoctorSpecialist != specialistId)
            {
                appointmentError += "This specialist is not suitable for this doctor. ";
                isAppointmentError = true;
            }
            else
            {
                switch (requestedTime)
                {
                    case 1:
                        requestedDate = requestedDate.Date.AddHours(7);
                        break;
                    case 2:
                        requestedDate = requestedDate.Date.AddHours(8);
                        break;
                    case 3:
                        requestedDate = requestedDate.Date.AddHours(9);
                        break;
                    case 4:
                        requestedDate = requestedDate.Date.AddHours(10);
                        break;
                    case 5:
                        requestedDate = requestedDate.Date.AddHours(13);
                        break;
                    case 6:
                        requestedDate = requestedDate.Date.AddHours(14);
                        break;
                    case 7:
                        requestedDate = requestedDate.Date.AddHours(15);
                        break;
                    case 8:
                        requestedDate = requestedDate.Date.AddHours(16);
                        break;

                }
                var appointment = _context.Appointments.FirstOrDefault(a => a.DoctorId == doctorId && a.RequestedTime.Equals(DateTime.Parse(requestedDate.ToString("yyyy-MM-dd HH:mm:ss"))) && a.Status == 1);
                if (appointment != null)
                {
                    appointmentError += "The doctor already has an appointment at this time. ";
                    isAppointmentError = true;
                }
                else
                {
                    newAppointment.Description = symptoms;
                    newAppointment.DoctorId = doctorId;
                    newAppointment.BranchId = branchId;
                    newAppointment.Specialist = specialistId;
                    newAppointment.RequestedTime = DateTime.Parse(requestedDate.ToString("yyyy-MM-dd HH:mm:ss"));
                }

            }

            if (isAppointmentError || isPatientError)
            {
                errorMessage = "Error occurs";
                return Page();
            }
            else
            {
                string token = HttpContext.Request.Cookies["AuthToken"];
                User u = authentication.GetUserFromToken(token);

                newAppointment.ReceptionistId = u.UserId;

                newAppointment.CreatedAt = DateTime.Now;
                newAppointment.Status = 1;
                newAppointment.CreatedAt = DateTime.Now;
                _context.Appointments.Add(newAppointment);
                _context.SaveChanges();
                string activeLink = _config["Host"] + _config["Port"] + "/PatientAppointment/Details?id=" + newAppointment.PatientId;
                var htmlContent = await _emailService.GetAppointmentApprovedEmail("appointment_approved.html", newAppointment.Branch.BranchName, newAppointment.PatientName, newAppointment.PatientAddress, newAppointment.PatientDob.ToString(), newAppointment.PatientPhoneNumber, newAppointment.PatientEmail, newAppointment.RequestedTime.ToString(), newAppointment.SpecialistNavigation.SpecialistName, newAppointment.Description, activeLink, newAppointment.Doctor.Name, newAppointment.Receptionist.Name, "Approved");
                _emailService.SendEmailAppointment(newAppointment.PatientEmail, "[Appointment Approved] Your Appointment Has Been Approved", htmlContent);
                if(newAppointment.PatientId != null)
                {
                    _notificationService.SendAppointmentNotification((int)newAppointment.PatientId, "Your appointment has been approved", activeLink);
                }
                return RedirectToPage("./Index", new { Message = "Appointment created!" });

            }
            //return Page();
            //return RedirectToPage("./Index");
        }
    }
}
