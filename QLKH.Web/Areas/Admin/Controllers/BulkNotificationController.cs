using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QLKH.Application.DTOs;
using QLKH.Application.Interfaces.Messaging;
using QLKH.Application.Interfaces.Services;
using QLKH.Application.Messages;
using QLKH.Infrastructure.Identity;
using QLKH.Web.Areas.Admin.Models;

namespace QLKH.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BulkNotificationController : Controller
    {
        private readonly IBulkNotificationService _bulkNotificationService;
        private readonly IAdminAuditLogService _adminAuditLogService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMessagePublisher _messagePublisher;

        public BulkNotificationController(
            IBulkNotificationService bulkNotificationService,
            IAdminAuditLogService adminAuditLogService,
            UserManager<ApplicationUser> userManager,
            IMessagePublisher messagePublisher)
        {
            _bulkNotificationService = bulkNotificationService;
            _adminAuditLogService = adminAuditLogService;
            _userManager = userManager;
            _messagePublisher = messagePublisher;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new BulkNotificationViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(BulkNotificationViewModel model)
        {
            if (!model.SendToStudents && !model.SendToTeachers)
            {
                ModelState.AddModelError(string.Empty, "Vui lòng chọn ít nhất một nhóm nhận thông báo.");
            }

            List<EmailAttachmentDto>? attachments = null;

            if (model.AttachmentFile != null && model.AttachmentFile.Length > 0)
            {
                const long maxSize = 10 * 1024 * 1024; // 10MB

                if (model.AttachmentFile.Length > maxSize)
                {
                    ModelState.AddModelError(nameof(model.AttachmentFile), "Tệp đính kèm không được vượt quá 10MB.");
                }
                else
                {
                    using var ms = new MemoryStream();
                    await model.AttachmentFile.CopyToAsync(ms);

                    attachments = new List<EmailAttachmentDto>
                    {
                        new EmailAttachmentDto
                        {
                            FileName = model.AttachmentFile.FileName,
                            ContentType = string.IsNullOrWhiteSpace(model.AttachmentFile.ContentType)
                                ? "application/octet-stream"
                                : model.AttachmentFile.ContentType,
                            Content = ms.ToArray()
                        }
                    };
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (sentCount, failedCount) = await _bulkNotificationService.SendAsync(
                model.Subject,
                model.Message,
                model.SendToStudents,
                model.SendToTeachers,
                attachments);

            if (sentCount == 0 && failedCount == 0)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người nhận hợp lệ. Hãy kiểm tra học viên/giáo viên đã liên kết tài khoản, có email và chưa bị khóa.";
                return RedirectToAction(nameof(Index));
            }

            var targetText = model.SendToStudents && model.SendToTeachers
                ? "Học viên + Giáo viên"
                : model.SendToStudents
                    ? "Học viên"
                    : "Giáo viên";

            var attachmentFileName = attachments != null && attachments.Any()
                ? attachments.First().FileName
                : null;

            var note = $"Tiêu đề: {model.Subject} | Gửi thành công: {sentCount} | Thất bại: {failedCount}";

            if (!string.IsNullOrWhiteSpace(attachmentFileName))
            {
                note += $" | File đính kèm: {attachmentFileName}";
            }

            await _adminAuditLogService.WriteAsync(
                CurrentAdminId,
                CurrentAdminEmail,
                "SEND_BULK_NOTIFICATION",
                "Notification",
                null,
                targetText,
                note);

            var notificationMessage = new AdminNotificationMessage
            {
                Subject = model.Subject,
                Message = model.Message,
                TargetText = targetText,
                SentCount = sentCount,
                FailedCount = failedCount,
                AttachmentFileName = attachmentFileName,
                CreatedBy = CurrentAdminEmail,
                CreatedAt = DateTime.Now
            };

            _messagePublisher.Publish("admin-notification-queue", notificationMessage);

            TempData["SuccessMessage"] = $"Đã gửi thành công {sentCount} email. Thất bại {failedCount} email.";
            return RedirectToAction(nameof(Index));
        }

        private string CurrentAdminEmail => User.Identity?.Name ?? "Unknown Admin";

        private string? CurrentAdminId => _userManager.GetUserId(User);
    }
}