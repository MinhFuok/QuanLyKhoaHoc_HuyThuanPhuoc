using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using QLKH.Web.Models;

namespace QLKH.Web.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly EmailSettings _emailSettings;

        public EmailSenderService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new InvalidOperationException("Email người nhận đang rỗng.");

            if (string.IsNullOrWhiteSpace(_emailSettings.SenderEmail))
                throw new InvalidOperationException("SenderEmail trong EmailSettings đang rỗng.");

            if (string.IsNullOrWhiteSpace(_emailSettings.Username))
                throw new InvalidOperationException("Username trong EmailSettings đang rỗng.");

            if (string.IsNullOrWhiteSpace(_emailSettings.Password))
                throw new InvalidOperationException("Password trong EmailSettings đang rỗng.");
            using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
            {
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                EnableSsl = true
            };

            using var message = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);

            await client.SendMailAsync(message);
        }
    }
}