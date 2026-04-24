using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QLKH.Application.Interfaces.Services;
using QLKH.Web.Models;

namespace QLKH.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISystemSettingService _systemSettingService;
        private readonly IHomeBannerSlideService _homeBannerSlideService;

        public HomeController(
            ILogger<HomeController> logger,
            ISystemSettingService systemSettingService,
            IHomeBannerSlideService homeBannerSlideService)
        {
            _logger = logger;
            _systemSettingService = systemSettingService;
            _homeBannerSlideService = homeBannerSlideService;
        }

        public async Task<IActionResult> Index()
        {
            var systemSetting = await _systemSettingService.GetAsync();
            var bannerSlides = await _homeBannerSlideService.GetActiveSlidesAsync();

            var model = new HomePageViewModel
            {
                SystemSetting = systemSetting,
                BannerSlides = bannerSlides
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        public async Task<IActionResult> Maintenance()
        {
            var setting = await _systemSettingService.GetAsync();

            ViewBag.MaintenanceMessage =
                !string.IsNullOrWhiteSpace(setting?.MaintenanceMessage)
                    ? setting!.MaintenanceMessage
                    : "Hệ thống đang bảo trì. Vui lòng quay lại sau.";

            return View();
        }
        public IActionResult About()
        {
            return View();
        }
    }
}