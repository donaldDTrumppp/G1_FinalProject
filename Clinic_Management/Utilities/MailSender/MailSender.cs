
using System.Net;
using System.Net.Mail;

namespace Clinic_Management.Utilities.MailSender
{
    public class MailSender : IMailSender
    {
        public Task SendMailAsync(string email, string subject, string message)
        {
            var mail = "";
            var pw = "";

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pw)
            };

            return client.SendMailAsync
                (new MailMessage(from: mail,
                                to: email,
                                subject,
                                message));
        }
    }
}
