using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace Clinic_Management.Services
{
    public class EmailService
    {

        private readonly IConfiguration _config;

        private readonly IWebHostEnvironment _env;

        public EmailService(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }

        public Task SendEmailAppointment(string to, string subject, string htmlContent)
        {
            var client = new SmtpClient(_config["SmtpSettings:Server"], int.Parse(_config["SmtpSettings:Port"]))
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(_config["SmtpSettings:SenderEmail"], _config["SmtpSettings:Password"])
            };
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["SmtpSettings:SenderEmail"]),
                Subject = subject,
                Body = htmlContent,
                IsBodyHtml = true
            };
            mailMessage.To.Add(to);
            LinkedResource linkedResource = new LinkedResource("wwwroot/Images/appointment_detail.png", MediaTypeNames.Image.Jpeg);
            linkedResource.ContentId = "image";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlContent, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(linkedResource);
            mailMessage.AlternateViews.Add(alternateView);

            linkedResource = new LinkedResource("wwwroot/Images/footer.png", MediaTypeNames.Image.Jpeg);
            linkedResource.ContentId = "footer";
            alternateView = AlternateView.CreateAlternateViewFromString(htmlContent, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(linkedResource);
            mailMessage.AlternateViews.Add(alternateView);

            return client.SendMailAsync(mailMessage);
        }

        public Task SendEmailNoHeader(string to, string subject, string htmlContent)
        {
            var client = new SmtpClient(_config["SmtpSettings:Server"], int.Parse(_config["SmtpSettings:Port"]))
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(_config["SmtpSettings:SenderEmail"], _config["SmtpSettings:Password"])
            };
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["SmtpSettings:SenderEmail"]),
                Subject = subject,
                Body = htmlContent,
                IsBodyHtml = true
            };
            mailMessage.To.Add(to);
  
            LinkedResource linkedResource = new LinkedResource("wwwroot/Images/footer.png", MediaTypeNames.Image.Jpeg);
            linkedResource.ContentId = "footer";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlContent, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(linkedResource);
            mailMessage.AlternateViews.Add(alternateView);

            return client.SendMailAsync(mailMessage);
        }

        public Task SendEmailMedical(string to, string subject, string htmlContent)
        {
            var client = new SmtpClient(_config["SmtpSettings:Server"], int.Parse(_config["SmtpSettings:Port"]))
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(_config["SmtpSettings:SenderEmail"], _config["SmtpSettings:Password"])
            };
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["SmtpSettings:SenderEmail"]),
                Subject = subject,
                Body = htmlContent,
                IsBodyHtml = true
            };
            mailMessage.To.Add(to);
            LinkedResource linkedResource = new LinkedResource("wwwroot/Images/heading-notext.png", MediaTypeNames.Image.Jpeg);
            linkedResource.ContentId = "image";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlContent, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(linkedResource);
            mailMessage.AlternateViews.Add(alternateView);

            linkedResource = new LinkedResource("wwwroot/Images/footer.png", MediaTypeNames.Image.Jpeg);
            linkedResource.ContentId = "footer";
            alternateView = AlternateView.CreateAlternateViewFromString(htmlContent, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(linkedResource);
            mailMessage.AlternateViews.Add(alternateView);
            Console.WriteLine("Email Sended to " + to);
            return client.SendMailAsync(mailMessage);
        }

        public async Task<string> GetAppointmentCreatedEmail(string fileName, string branchName, string patientName, string patientAddress, string patientDob, string patientPhone, string patientEmail, string requestedTime, string specialist, string description, string activeLink)
        {
            var filePath = Path.Combine(_env.WebRootPath, "template", fileName);
            var htmlContent = await File.ReadAllTextAsync(filePath);
            htmlContent = htmlContent.Replace("<span id=\"dname\" style=\"font-weight: bold\"></span>", $"<span id=\"dname\" style=\"font-weight: bold\">{patientName}</span>");
            htmlContent = htmlContent.Replace("<span id=\"name\" style=\"font-weight: bold\"></span>", $"<span id=\"dname\" style=\"font-weight: bold\">{patientName}</span>");
            htmlContent = htmlContent.Replace("<span id=\"branch\" style=\"font-weight: bold\"></span>", $"<span id=\"branch\" style=\"font-weight: bold\">{branchName}</span>");
            htmlContent = htmlContent.Replace("<span id=\"name\" style=\"font-weight: bold\"></span>", $"<span id=\"name\" style=\"font-weight: bold\">{patientName}</span>");
            htmlContent = htmlContent.Replace("<span id=\"address\" style=\"font-weight: bold\"></span>", $"<span id=\"address\" style=\"font-weight: bold\">{patientAddress}</span>");
            htmlContent = htmlContent.Replace("<span id=\"dob\" style=\"font-weight: bold\"></span>", $"<span id=\"dob\" style=\"font-weight: bold\">{patientDob}</span>");
            htmlContent = htmlContent.Replace("<span id=\"phone\" style=\"font-weight: bold\"></span>", $"<span id=\"phone\" style=\"font-weight: bold\">{patientPhone}</span>");
            htmlContent = htmlContent.Replace("<span id=\"email\" style=\"font-weight: bold\"></span>", $"<span id=\"email\" style=\"font-weight: bold\">{patientEmail}</span>");
            htmlContent = htmlContent.Replace("<span id=\"time\" style=\"font-weight: bold\"></span>", $"<span id=\"time\" style=\"font-weight: bold\">{requestedTime}</span>");
            htmlContent = htmlContent.Replace("<span id=\"specialist\" style=\"font-weight: bold\"></span>", $"<span id=\"specialist\" style=\"font-weight: bold\">{specialist}</span>");
            htmlContent = htmlContent.Replace("<span id=\"description\" style=\"font-weight: bold\"></span>", $"<span id=\"description\" style=\"font-weight: bold\">{description}</span>");
            htmlContent = htmlContent.Replace("<a href=\"\">here.</a>", $"<a href=\"{activeLink}\">here.</a>");
            return htmlContent;
        }

        public async Task<string> GetAppointmentEditedEmail(string fileName, string branchName, string patientName, string patientAddress, string patientDob, string patientPhone, string patientEmail, string requestedTime, string specialist, string description, string activeLink, string doctorName, string receptionistName, string status)
        {
            var filePath = Path.Combine(_env.WebRootPath, "template", fileName);
            var htmlContent = await File.ReadAllTextAsync(filePath);
            htmlContent = htmlContent.Replace("<span id=\"status\" style=\"font-weight: bold\"></span>", $"<span id=\"status\" style=\"font-weight: bold\">{status}</span>");
            htmlContent = htmlContent.Replace("<span id=\"doctor\" style=\"font-weight: bold\"></span>", $"<span id=\"doctor\" style=\"font-weight: bold\">{doctorName}</span>");
            htmlContent = htmlContent.Replace("<span id=\"receptionist\" style=\"font-weight: bold\"></span>", $"<span id=\"receptionist\" style=\"font-weight: bold\">{receptionistName}</span>");
            htmlContent = htmlContent.Replace("<span id=\"dname\" style=\"font-weight: bold\"></span>", $"<span id=\"dname\" style=\"font-weight: bold\">{patientName}</span>");
            htmlContent = htmlContent.Replace("<span id=\"branch\" style=\"font-weight: bold\"></span>", $"<span id=\"branch\" style=\"font-weight: bold\">{branchName}</span>");
            htmlContent = htmlContent.Replace("<span id=\"name\" style=\"font-weight: bold\"></span>", $"<span id=\"name\" style=\"font-weight: bold\">{patientName}</span>");
            htmlContent = htmlContent.Replace("<span id=\"address\" style=\"font-weight: bold\"></span>", $"<span id=\"address\" style=\"font-weight: bold\">{patientAddress}</span>");
            htmlContent = htmlContent.Replace("<span id=\"dob\" style=\"font-weight: bold\"></span>", $"<span id=\"dob\" style=\"font-weight: bold\">{patientDob}</span>");
            htmlContent = htmlContent.Replace("<span id=\"phone\" style=\"font-weight: bold\"></span>", $"<span id=\"phone\" style=\"font-weight: bold\">{patientPhone}</span>");
            htmlContent = htmlContent.Replace("<span id=\"email\" style=\"font-weight: bold\"></span>", $"<span id=\"email\" style=\"font-weight: bold\">{patientEmail}</span>");
            htmlContent = htmlContent.Replace("<span id=\"time\" style=\"font-weight: bold\"></span>", $"<span id=\"time\" style=\"font-weight: bold\">{requestedTime}</span>");
            htmlContent = htmlContent.Replace("<span id=\"specialist\" style=\"font-weight: bold\"></span>", $"<span id=\"specialist\" style=\"font-weight: bold\">{specialist}</span>");
            htmlContent = htmlContent.Replace("<span id=\"description\" style=\"font-weight: bold\"></span>", $"<span id=\"description\" style=\"font-weight: bold\">{description}</span>");
            htmlContent = htmlContent.Replace("<a href=\"\">here.</a>", $"<a href=\"{activeLink}\">here.</a>");
            return htmlContent;
        }

        public async Task<string> GetAppointmentCancelledEmail(string fileName, string branchName, string requestedTime, string patientName)
        {
            var filePath = Path.Combine(_env.WebRootPath, "template", fileName);
            var htmlContent = await File.ReadAllTextAsync(filePath);
            htmlContent = htmlContent.Replace("<span id=\"dname\" style=\"font-weight: bold\"></span>", $"<span id=\"dname\" style=\"font-weight: bold\">{patientName}</span>");
            htmlContent = htmlContent.Replace("<span id=\"branch\" style=\"font-weight: bold\"></span>", $"<span id=\"branch\" style=\"font-weight: bold\">{branchName}</span>");
            htmlContent = htmlContent.Replace("<span id=\"time\" style=\"font-weight: bold\"></span>", $"<span id=\"time\" style=\"font-weight: bold\">{requestedTime}</span>");
            return htmlContent;
        }

        public async Task<string> GetRegisterEmail(string fileName, string activeLink, string patientName)
        {
            var filePath = Path.Combine(_env.WebRootPath, "template", fileName);
            var htmlContent = await File.ReadAllTextAsync(filePath);
            htmlContent = htmlContent.Replace("<span id=\"dname\" style=\"font-weight: bold\"></span>", $"<span id=\"dname\" style=\"font-weight: bold\">{patientName}</span>");
            htmlContent = htmlContent.Replace("<a href=\"\" style=\"color: white !important; text-decoration: none\">Verify Account</a>", $"<a href=\"{activeLink}\" style=\"color: white !important; text-decoration: none\">Verify Account</a>");
            return htmlContent;
        }

        public async Task<string> GetAddUserEmail(string fileName, string activeLink, string patientName)
        {
            var filePath = Path.Combine(_env.WebRootPath, "template", fileName);
            var htmlContent = await File.ReadAllTextAsync(filePath);
            htmlContent = htmlContent.Replace("<span id=\"dname\" style=\"font-weight: bold\"></span>", $"<span id=\"dname\" style=\"font-weight: bold\">{patientName}</span>");
            htmlContent = htmlContent.Replace("<a href=\"\" style=\"color: white !important; text-decoration: none\">Verify Account</a>", $"<a href=\"{activeLink}\" style=\"color: white !important; text-decoration: none\">Verify Account</a>");
            return htmlContent;
        }

        public async Task<string> GetEditUserEmail(string fileName, string patientName, string patientAddress, string patientDob, string patientPhone, string patientEmail, string activeLink)
        {
            var filePath = Path.Combine(_env.WebRootPath, "template", fileName);
            var htmlContent = await File.ReadAllTextAsync(filePath);
            htmlContent = htmlContent.Replace("<span id=\"dname\" style=\"font-weight: bold\"></span>", $"<span id=\"dname\" style=\"font-weight: bold\">{patientName}</span>");
            htmlContent = htmlContent.Replace("<span id=\"name\" style=\"font-weight: bold\"></span>", $"<span id=\"name\" style=\"font-weight: bold\">{patientName}</span>");
            htmlContent = htmlContent.Replace("<span id=\"address\" style=\"font-weight: bold\"></span>", $"<span id=\"address\" style=\"font-weight: bold\">{patientAddress}</span>");
            htmlContent = htmlContent.Replace("<span id=\"dob\" style=\"font-weight: bold\"></span>", $"<span id=\"dob\" style=\"font-weight: bold\">{patientDob}</span>");
            htmlContent = htmlContent.Replace("<span id=\"phone\" style=\"font-weight: bold\"></span>", $"<span id=\"phone\" style=\"font-weight: bold\">{patientPhone}</span>");
            htmlContent = htmlContent.Replace("<span id=\"email\" style=\"font-weight: bold\"></span>", $"<span id=\"email\" style=\"font-weight: bold\">{patientEmail}</span>");
            htmlContent = htmlContent.Replace("<a href=\"\">here.</a>", $"<a href=\"{activeLink}\">here.</a>");
            return htmlContent;
        }

        public async Task<string> GetForgotPasswordEmail(string fileName, string activeLink, string patientName)
        {
            var filePath = Path.Combine(_env.WebRootPath, "template", fileName);
            var htmlContent = await File.ReadAllTextAsync(filePath);
            htmlContent = htmlContent.Replace("<span id=\"dname\" style=\"font-weight: bold\"></span>", $"<span id=\"dname\" style=\"font-weight: bold\">{patientName}</span>");
            htmlContent = htmlContent.Replace("<a href=\"\" style=\"color: white !important; text-decoration: none\">Reset Password</a>", $"<a href=\"{activeLink}\" style=\"color: white !important; text-decoration: none\">Reset Password</a>");
            return htmlContent;
        }

        public async Task<string> GetMedicalRecordEmail(string fileName, string branchName, string patientName, string patientAddress, string patientDob, string patientPhone, string patientEmail, string symptoms, string diagnosis, string prescription, string activeLink, string doctorName, DateTime visitTime)
        {
            var filePath = Path.Combine(_env.WebRootPath, "template", fileName);
            var htmlContent = await File.ReadAllTextAsync(filePath);
            htmlContent = htmlContent.Replace("<span id=\"time\" style=\"font-weight: bold\"></span>", $"<span id=\"doctor\" style=\"font-weight: bold\">{visitTime}</span>");
            htmlContent = htmlContent.Replace("<span id=\"doctor\" style=\"font-weight: bold\"></span>", $"<span id=\"doctor\" style=\"font-weight: bold\">{doctorName}</span>");
            htmlContent = htmlContent.Replace("<span id=\"dname\" style=\"font-weight: bold\"></span>", $"<span id=\"dname\" style=\"font-weight: bold\">{patientName}</span>");
            htmlContent = htmlContent.Replace("<span id=\"branch\" style=\"font-weight: bold\"></span>", $"<span id=\"branch\" style=\"font-weight: bold\">{branchName}</span>");
            htmlContent = htmlContent.Replace("<span id=\"name\" style=\"font-weight: bold\"></span>", $"<span id=\"name\" style=\"font-weight: bold\">{patientName}</span>");
            htmlContent = htmlContent.Replace("<span id=\"address\" style=\"font-weight: bold\"></span>", $"<span id=\"address\" style=\"font-weight: bold\">{patientAddress}</span>");
            htmlContent = htmlContent.Replace("<span id=\"dob\" style=\"font-weight: bold\"></span>", $"<span id=\"dob\" style=\"font-weight: bold\">{patientDob}</span>");
            htmlContent = htmlContent.Replace("<span id=\"phone\" style=\"font-weight: bold\"></span>", $"<span id=\"phone\" style=\"font-weight: bold\">{patientPhone}</span>");
            htmlContent = htmlContent.Replace("<span id=\"email\" style=\"font-weight: bold\"></span>", $"<span id=\"email\" style=\"font-weight: bold\">{patientEmail}</span>");
            htmlContent = htmlContent.Replace("<span id=\"symptoms\" style=\"font-weight: bold\"></span>", $"<span id=\"symptoms\" style=\"font-weight: bold\">{symptoms}</span>");
            htmlContent = htmlContent.Replace("<span id=\"diagnosis\" style=\"font-weight: bold\"></span>", $"<span id=\"diagnosis\" style=\"font-weight: bold\">{diagnosis}</span>");
            htmlContent = htmlContent.Replace("<span id=\"prescription\" style=\"font-weight: bold\"></span>", $"<span id=\"prescription\" style=\"font-weight: bold\">{prescription}</span>");
            htmlContent = htmlContent.Replace("<a href=\"\">here.</a>", $"<a href=\"{activeLink}\">here.</a>");
            return htmlContent;
        }

        public async Task<string> GetHtmlContentAsync(string fileName)
        {
            var filePath = Path.Combine(_env.WebRootPath, "template", fileName);
            var htmlContent = await File.ReadAllTextAsync(filePath);
            return htmlContent;
        }


    }
}
