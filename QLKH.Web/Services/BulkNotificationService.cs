using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QLKH.Application.DTOs;
using QLKH.Application.Interfaces.Services;
using QLKH.Infrastructure.Identity;

namespace QLKH.Web.Services
{
    public class BulkNotificationService : IBulkNotificationService
    {
        private readonly IStudentService _studentService;
        private readonly ITeacherService _teacherService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSenderService _emailSenderService;

        public BulkNotificationService(
            IStudentService studentService,
            ITeacherService teacherService,
            UserManager<ApplicationUser> userManager,
            IEmailSenderService emailSenderService)
        {
            _studentService = studentService;
            _teacherService = teacherService;
            _userManager = userManager;
            _emailSenderService = emailSenderService;
        }

        public async Task<(int sentCount, int failedCount)> SendAsync(
            string subject,
            string message,
            bool sendToStudents,
            bool sendToTeachers,
            List<EmailAttachmentDto>? attachments = null)
        {
            var emailSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (sendToStudents)
            {
                var students = await _studentService.GetAllAsync();
                foreach (var student in students)
                {
                    if (string.IsNullOrWhiteSpace(student.ApplicationUserId))
                        continue;

                    var user = await _userManager.Users
                        .FirstOrDefaultAsync(x => x.Id == student.ApplicationUserId);

                    if (user == null)
                        continue;

                    var isLocked = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow;
                    if (isLocked)
                        continue;

                    if (!string.IsNullOrWhiteSpace(user.Email))
                    {
                        emailSet.Add(user.Email);
                    }
                }
            }

            if (sendToTeachers)
            {
                var teachers = await _teacherService.GetAllAsync();
                foreach (var teacher in teachers)
                {
                    if (string.IsNullOrWhiteSpace(teacher.ApplicationUserId))
                        continue;

                    var user = await _userManager.Users
                        .FirstOrDefaultAsync(x => x.Id == teacher.ApplicationUserId);

                    if (user == null)
                        continue;

                    var isLocked = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow;
                    if (isLocked)
                        continue;

                    if (!string.IsNullOrWhiteSpace(user.Email))
                    {
                        emailSet.Add(user.Email);
                    }
                }
            }

            int sentCount = 0;
            int failedCount = 0;

            var htmlBody = $@"
<div style='font-family:Arial,Helvetica,sans-serif; font-size:14px; line-height:1.6; color:#222; white-space:pre-line;'>
    {System.Net.WebUtility.HtmlEncode(message)}
</div>";

            foreach (var email in emailSet)
            {
                try
                {
                    await _emailSenderService.SendEmailAsync(email, subject, htmlBody, attachments);
                    sentCount++;
                }
                catch
                {
                    failedCount++;
                }
            }

            return (sentCount, failedCount);
        }
    }
}