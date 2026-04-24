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

                vm.FeatureCard1Number = setting.FeatureCard1Number ?? "01";
                vm.FeatureCard1Title = setting.FeatureCard1Title ?? "Quản lý tập trung";
                vm.FeatureCard1Description = setting.FeatureCard1Description ?? "khóa học • lớp học • học viên";

                vm.FeatureCard2Number = setting.FeatureCard2Number ?? "02";
                vm.FeatureCard2Title = setting.FeatureCard2Title ?? "Dễ vận hành";
                vm.FeatureCard2Description = setting.FeatureCard2Description ?? "học vụ • quản trị • cấu hình";

                vm.FeatureCard3Number = setting.FeatureCard3Number ?? "03";
                vm.FeatureCard3Title = setting.FeatureCard3Title ?? "Học online";
                vm.FeatureCard3Description = setting.FeatureCard3Description ?? "tài liệu • bài tập • tiến độ";

                vm.FeatureCard4Number = setting.FeatureCard4Number ?? "04";
                vm.FeatureCard4Title = setting.FeatureCard4Title ?? "Mở rộng dễ dàng";
                vm.FeatureCard4Description = setting.FeatureCard4Description ?? "báo cáo • nghiệp vụ • dữ liệu";
            }
            else
            {
                vm.FeatureCard1Number = "01";
                vm.FeatureCard1Title = "Quản lý tập trung";
                vm.FeatureCard1Description = "khóa học • lớp học • học viên";

                vm.FeatureCard2Number = "02";
                vm.FeatureCard2Title = "Dễ vận hành";
                vm.FeatureCard2Description = "học vụ • quản trị • cấu hình";

                vm.FeatureCard3Number = "03";
                vm.FeatureCard3Title = "Học online";
                vm.FeatureCard3Description = "tài liệu • bài tập • tiến độ";

                vm.FeatureCard4Number = "04";
                vm.FeatureCard4Title = "Mở rộng dễ dàng";
                vm.FeatureCard4Description = "báo cáo • nghiệp vụ • dữ liệu";
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

            setting.FeatureCard1Number = model.FeatureCard1Number;
            setting.FeatureCard1Title = model.FeatureCard1Title;
            setting.FeatureCard1Description = model.FeatureCard1Description;

            setting.FeatureCard2Number = model.FeatureCard2Number;
            setting.FeatureCard2Title = model.FeatureCard2Title;
            setting.FeatureCard2Description = model.FeatureCard2Description;

            setting.FeatureCard3Number = model.FeatureCard3Number;
            setting.FeatureCard3Title = model.FeatureCard3Title;
            setting.FeatureCard3Description = model.FeatureCard3Description;

            setting.FeatureCard4Number = model.FeatureCard4Number;
            setting.FeatureCard4Title = model.FeatureCard4Title;
            setting.FeatureCard4Description = model.FeatureCard4Description;

            await _systemSettingService.UpdateAsync(setting);

            await _adminAuditLogService.WriteAsync(
                CurrentAdminId,
                CurrentAdminEmail,
                "UPDATE_SYSTEM_SETTINGS",
                "SystemSetting",
                setting.Id.ToString(),
                "Cấu hình hệ thống",
                "Cập nhật giao diện website / liên hệ / email / 4 thẻ trang chủ");

            TempData["SuccessMessage"] = "Cập nhật cấu hình hệ thống thành công.";
            return RedirectToAction(nameof(Index));
        }

        private string CurrentAdminEmail => User.Identity?.Name ?? "Unknown Admin";
        private string? CurrentAdminId => _userManager.GetUserId(User);
    }
}