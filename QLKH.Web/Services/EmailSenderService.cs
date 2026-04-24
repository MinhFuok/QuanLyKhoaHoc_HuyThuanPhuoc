using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using QLKH.Application.DTOs;
using QLKH.Application.Interfaces.Services;
using QLKH.Web.Models;

namespace QLKH.Web.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ISystemSettingService _systemSettingService;

        public EmailSenderService(
            IOptions<EmailSettings> emailSettings,
            ISystemSettingService systemSettingService)
        {
            _emailSettings = emailSettings.Value;
            _systemSettingService = systemSettingService;
        }

        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string htmlMessage,
            List<EmailAttachmentDto>? attachments = null)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new InvalidOperationException("Email người nhận đang rỗng.");

            var systemSetting = await _systemSettingService.GetAsync();

            var enableEmail = systemSetting?.EnableEmail ?? true;
            if (!enableEmail)
                throw new InvalidOperationException("Chức năng gửi email hiện đang bị tắt trong cấu hình hệ thống.");

            var smtpServer = !string.IsNullOrWhiteSpace(systemSetting?.SmtpServer)
                ? systemSetting!.SmtpServer!
                : _emailSettings.SmtpServer;

            var smtpPort = systemSetting?.SmtpPort > 0
                ? systemSetting!.SmtpPort
                : _emailSettings.Port;

            var senderName = !string.IsNullOrWhiteSpace(systemSetting?.SenderName)
                ? systemSetting!.SenderName!
                : _emailSettings.SenderName;

            var senderEmail = !string.IsNullOrWhiteSpace(systemSetting?.SenderEmail)
                ? systemSetting!.SenderEmail!
                : _emailSettings.SenderEmail;

            var smtpUsername = !string.IsNullOrWhiteSpace(systemSetting?.SmtpUsername)
                ? systemSetting!.SmtpUsername!
                : _emailSettings.Username;

            var smtpPassword = !string.IsNullOrWhiteSpace(systemSetting?.SmtpPassword)
                ? systemSetting!.SmtpPassword!
                : _emailSettings.Password;

            if (string.IsNullOrWhiteSpace(senderEmail))
                throw new InvalidOperationException("SenderEmail đang rỗng.");

            if (string.IsNullOrWhiteSpace(smtpUsername))
                throw new InvalidOperationException("SMTP Username đang rỗng.");

            if (string.IsNullOrWhiteSpace(smtpPassword))
                throw new InvalidOperationException("SMTP Password đang rỗng.");

            if (string.IsNullOrWhiteSpace(smtpServer))
                throw new InvalidOperationException("SMTP Server đang rỗng.");

            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true
            };

            using var message = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);

            if (attachments != null && attachments.Any())
            {
                foreach (var file in attachments)
                {
                    var stream = new MemoryStream(file.Content);
                    var attachment = new Attachment(stream, file.FileName, file.ContentType);
                    message.Attachments.Add(attachment);
                }
            }

            await client.SendMailAsync(message);
        }
    }
}