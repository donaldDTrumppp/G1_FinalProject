namespace Clinic_Management.Utilities.MailSender
{
    public interface IMailSender
    {
        Task SendMailAsync(string email, string subject, string message);
    }
}
