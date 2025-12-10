using Buisness_Logic_Layer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Buisness_Logic_Layer.Repositories
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using (var client = new SmtpClient("smtp.example.com", 587))
            {
                client.Credentials = new NetworkCredential("your_email@example.com", "your_password");
                client.EnableSsl = true;

                var mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("your_email@example.com");
                mailMessage.To.Add(toEmail);
                mailMessage.Subject = subject;
                mailMessage.Body = body;

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
