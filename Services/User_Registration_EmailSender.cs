using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using DotNetEnv;

namespace Product_Config_Customer_v0.Services
{
    public interface IEmailSender
    {
        Task SendAsync(string to, string subject, string body);
    }

    public class SmtpEmailSender : IEmailSender
    {
        public async Task SendAsync(string to, string subject, string body)
        {
            Env.Load(); 

            // Read from environment variables
            var smtpServer = Environment.GetEnvironmentVariable("EMAIL_SMTP") ?? "smtp.gmail.com";
            var smtpPortStr = Environment.GetEnvironmentVariable("EMAIL_PORT") ?? "587";
            var username = Environment.GetEnvironmentVariable("EMAIL_USERNAME") ?? "";
            var password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD") ?? "";
            var from = Environment.GetEnvironmentVariable("EMAIL_FROM") ?? username;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new InvalidOperationException("SMTP username or password is missing.");

            if (!int.TryParse(smtpPortStr, out int smtpPort))
                smtpPort = 587; // default to 587

            using var smtp = new SmtpClient(smtpServer)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true,             // GoDaddy requires SSL/TLS
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            using var mail = new MailMessage
            {
                From = new MailAddress(from),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(to);

            try
            {
                await smtp.SendMailAsync(mail);
            }
            catch (SmtpException ex)
            {
                // Detailed logging for debugging
                Console.WriteLine($"SMTP Error: {ex.StatusCode} - {ex.Message}");
                throw;
            }
        }
    }
}
