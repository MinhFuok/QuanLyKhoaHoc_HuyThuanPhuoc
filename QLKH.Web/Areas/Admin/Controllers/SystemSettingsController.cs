using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QLKH.Application.Interfaces.Services;
using QLKH.Domain.Entities;
using QLKH.Infrastructure.Identity;
using QLKH.Web.Areas.Admin.Models;

namespace QLKH.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SystemSettingsController : Controller
    {
        private readonly ISystemSettingService _systemSettingService;
        private readonly IHomeBannerSlideService _homeBannerSlideService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAdminAuditLogService _adminAuditLogService;
        private readonly UserManager<ApplicationUser> _userManager;

        public SystemSettingsController(
            ISystemSettingService systemSettingService,
            IHomeBannerSlideService homeBannerSlideService,
            IWebHostEnvironment webHostEnvironment,
            IAdminAuditLogService adminAuditLogService,
            UserManager<ApplicationUser> userManager)
        {
            _systemSettingService = systemSettingService;
            _homeBannerSlideService = homeBannerSlideService;
            _webHostEnvironment = webHostEnvironment;
            _adminAuditLogService = adminAuditLogService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var setting = await _systemSettingService.GetAsync();
            var bannerSlides = await _homeBannerSlideService.GetAllAsync();

            var vm = new SystemSettingViewModel();

            if (setting != null)
            {
                vm.Id = setting.Id;
                vm.SiteName = setting.SiteName ?? string.Empty;
                vm.HomeBannerTitle = setting.HomeBannerTitle ?? string.Empty;
                vm.HomeBannerSubtitle = setting.HomeBannerSubtitle ?? string.Empty;
                vm.HomeBannerImageUrl = setting.HomeBannerImageUrl ?? string.Empty;
                vm.FooterText = setting.FooterText ?? string.Empty;

                vm.ContactEmail = setting.ContactEmail ?? string.Empty;
                vm.ContactPhone = setting.ContactPhone ?? string.Empty;
                vm.ContactAddress = setting.ContactAddress ?? string.Empty;
                vm.FacebookUrl = setting.FacebookUrl ?? string.Empty;
                vm.YoutubeUrl = setting.YoutubeUrl ?? string.Empty;
                vm.TiktokUrl = setting.TiktokUrl ?? string.Empty;
                vm.XUrl = setting.XUrl ?? string.Empty;

                vm.IsWebsiteEnabled = setting.IsWebsiteEnabled;
                vm.MaintenanceMessage = setting.MaintenanceMessage ?? string.Empty;

                vm.EnableEmail = setting.EnableEmail;
                vm.SmtpServer = setting.SmtpServer ?? string.Empty;
                vm.SmtpPort = setting.SmtpPort;
                vm.SenderName = setting.SenderName ?? string.Empty;
                vm.SenderEmail = setting.SenderEmail ?? string.Empty;
                vm.SmtpUsername = setting.SmtpUsername ?? string.Empty;
                vm.SmtpPassword = setting.SmtpPassword ?? string.Empty;
            }

            var pageVm = new SystemSettingsPageViewModel
            {
                SystemSetting = vm,
                BannerSlides = bannerSlides.ToList()
            };

            return View(pageVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SystemSettingsPageViewModel pageVm)
        {
            if (!ModelState.IsValid)
            {
                pageVm.BannerSlides = (await _homeBannerSlideService.GetAllAsync()).ToList();
                return View(pageVm);
            }

            var model = pageVm.SystemSetting;

            var setting = await _systemSettingService.GetAsync();
            if (setting == null)
            {
                setting = new SystemSetting();
            }

            setting.SiteName = model.SiteName;
            setting.HomeBannerTitle = model.HomeBannerTitle;
            setting.HomeBannerSubtitle = model.HomeBannerSubtitle;
            setting.FooterText = model.FooterText;

            setting.ContactEmail = model.ContactEmail;
            setting.ContactPhone = model.ContactPhone;
            setting.ContactAddress = model.ContactAddress;
            setting.FacebookUrl = model.FacebookUrl;
            setting.YoutubeUrl = model.YoutubeUrl;
            setting.TiktokUrl = model.TiktokUrl;
            setting.XUrl = model.XUrl;

            setting.IsWebsiteEnabled = model.IsWebsiteEnabled;
            setting.MaintenanceMessage = model.MaintenanceMessage;

            setting.EnableEmail = model.EnableEmail;
            setting.SmtpServer = model.SmtpServer;
            setting.SmtpPort = model.SmtpPort;
            setting.SenderName = model.SenderName;
            setting.SenderEmail = model.SenderEmail;
            setting.SmtpUsername = model.SmtpUsername;
            setting.SmtpPassword = model.SmtpPassword;

            if (model.BannerImageFile != null && model.BannerImageFile.Length > 0)
            {
                var extension = Path.GetExtension(model.BannerImageFile.FileName).ToLowerInvariant();
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("SystemSetting.BannerImageFile", "Chỉ chấp nhận file ảnh .jpg, .jpeg, .png, .webp");
                    pageVm.BannerSlides = (await _homeBannerSlideService.GetAllAsync()).ToList();
                    return View(pageVm);
                }

                var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "banners");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var fileName = $"banner_{Guid.NewGuid():N}{extension}";
                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.BannerImageFile.CopyToAsync(stream);
                }

                setting.HomeBannerImageUrl = $"/uploads/banners/{fileName}";
            }

            await _systemSettingService.UpdateAsync(setting);

            await _adminAuditLogService.WriteAsync(
                CurrentAdminId,
                CurrentAdminEmail,
                "UPDATE_SYSTEM_SETTINGS",
                "SystemSetting",
                setting.Id.ToString(),
                "Cấu hình hệ thống",
                "Cập nhật cấu hình website/email/liên hệ");

            TempData["SuccessMessage"] = "Cập nhật cấu hình hệ thống thành công.";
            return RedirectToAction(nameof(Index));
        }

        private string CurrentAdminEmail => User.Identity?.Name ?? "Unknown Admin";
        private string? CurrentAdminId => _userManager.GetUserId(User);
    }
}