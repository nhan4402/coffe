using System.Net;
using System.Net.Mail;

namespace Shopping_Coffee.Areas.Admin.Repository
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true, //bật bảo mật
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("louree2081@gmail.com", "seda bmzk eizg dntr")
            };

            return client.SendMailAsync(
                new MailMessage(from: "louree2081@gmail.com",
                                to: email,
                                subject,
                                message
                                ));
        }
    }
}
