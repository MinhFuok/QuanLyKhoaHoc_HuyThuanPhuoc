using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QLKH.Application.Interfaces.Services;

namespace QLKH.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminAuditLogController : Controller
    {
        private readonly IAdminAuditLogService _adminAuditLogService;

        public AdminAuditLogController(IAdminAuditLogService adminAuditLogService)
        {
            _adminAuditLogService = adminAuditLogService;
        }

        public async Task<IActionResult> Index(
            string? adminEmail,
            string? actionName,
            DateTime? fromDate,
            DateTime? toDate,
            string? keyword)
        {
            var logs = await _adminAuditLogService.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(adminEmail))
            {
                logs = logs
                    .Where(x => !string.IsNullOrWhiteSpace(x.ActorEmail) &&
                                x.ActorEmail.Contains(adminEmail, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(actionName))
            {
                logs = logs
                    .Where(x => x.ActionName.Equals(actionName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (fromDate.HasValue)
            {
                var from = fromDate.Value.Date;
                logs = logs.Where(x => x.CreatedAt.ToLocalTime().Date >= from).ToList();
            }

            if (toDate.HasValue)
            {
                var to = toDate.Value.Date;
                logs = logs.Where(x => x.CreatedAt.ToLocalTime().Date <= to).ToList();
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                logs = logs
                    .Where(x =>
                        (!string.IsNullOrWhiteSpace(x.TargetDisplay) &&
                         x.TargetDisplay.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrWhiteSpace(x.Note) &&
                         x.Note.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrWhiteSpace(x.TargetType) &&
                         x.TargetType.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            var allLogs = await _adminAuditLogService.GetAllAsync();

            ViewBag.AdminEmail = adminEmail;
            ViewBag.ActionName = actionName;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
            ViewBag.Keyword = keyword;

            ViewBag.ActionList = allLogs
                .Select(x => x.ActionName)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .Select(x => new SelectListItem
                {
                    Value = x,
                    Text = x
                })
                .ToList();

            return View(logs.OrderByDescending(x => x.CreatedAt).ToList());
        }
    }
}