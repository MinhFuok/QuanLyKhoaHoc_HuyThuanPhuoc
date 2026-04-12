using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKH.Application.Interfaces.Services;
using QLKH.Domain.Entities;
using QLKH.Web.Areas.Admin.Models;

namespace QLKH.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SystemSettingsController : Controller
    {
        private readonly ISystemSettingService _systemSettingService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SystemSettingsController(
            ISystemSettingService systemSettingService,
            IWebHostEnvironment webHostEnvironment)
        {
            _systemSettingService = systemSettingService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var setting = await _systemSettingService.GetAsync();

            if (setting == null)
            {
                setting = new SystemSetting();
            }

            var model = new SystemSettingViewModel
            {
                Id = setting.Id,
                SiteName = setting.SiteName,
                HomeBannerTitle = setting.HomeBannerTitle,
                HomeBannerSubtitle = setting.HomeBannerSubtitle,
                HomeBannerImageUrl = setting.HomeBannerImageUrl,
                FooterText = setting.FooterText,
                ContactEmail = setting.ContactEmail,
                ContactPhone = setting.ContactPhone,
                ContactAddress = setting.ContactAddress,
                FacebookUrl = setting.FacebookUrl,
                YoutubeUrl = setting.YoutubeUrl,
                TiktokUrl = setting.TiktokUrl,
                XUrl = setting.XUrl,
                IsWebsiteEnabled = setting.IsWebsiteEnabled,
                MaintenanceMessage = setting.MaintenanceMessage,
                EnableEmail = setting.EnableEmail,
                SmtpServer = setting.SmtpServer,
                SmtpPort = setting.SmtpPort,
                SenderName = setting.SenderName,
                SenderEmail = setting.SenderEmail,
                SmtpUsername = setting.SmtpUsername,
                SmtpPassword = setting.SmtpPassword,
                EnableVnPay = setting.EnableVnPay,
                EnableMomo = setting.EnableMomo
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SystemSettingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

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
            setting.EnableVnPay = model.EnableVnPay;
            setting.EnableMomo = model.EnableMomo;

            if (model.BannerImageFile != null && model.BannerImageFile.Length > 0)
            {
                var extension = Path.GetExtension(model.BannerImageFile.FileName).ToLower();

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("BannerImageFile", "Chỉ chấp nhận file ảnh .jpg, .jpeg, .png, .webp");
                    return View(model);
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
            TempData["SuccessMessage"] = "Cập nhật cấu hình hệ thống thành công.";

            return RedirectToAction(nameof(Index));
        }
    }
}