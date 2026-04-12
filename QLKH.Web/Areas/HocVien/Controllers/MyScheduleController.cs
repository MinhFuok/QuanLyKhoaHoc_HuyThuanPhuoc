using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKH.Application.Interfaces.Services;
using System.Security.Claims;

namespace QLKH.Web.Areas.HocVien.Controllers
{
    [Area("HocVien")]
    [Authorize(Roles = "HocVien")]
    public class MyScheduleController : Controller
    {
        private readonly IClassScheduleService _classScheduleService;

        public MyScheduleController(IClassScheduleService classScheduleService)
        {
            _classScheduleService = classScheduleService;
        }

        public async Task<IActionResult> Index()
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(applicationUserId))
            {
                return Challenge();
            }

            var schedules = await _classScheduleService.GetMyLearningScheduleAsync(applicationUserId);
            return View(schedules);
        }
    }
}