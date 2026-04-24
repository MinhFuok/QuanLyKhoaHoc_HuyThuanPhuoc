using QLKH.Application.DTOs;

namespace QLKH.Web.Services
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(
            string toEmail,
            string subject,
            string htmlMessage,
            List<EmailAttachmentDto>? attachments = null);
    }
}