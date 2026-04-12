using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKH.Application.Interfaces.Services;
using System.Security.Claims;

namespace QLKH.Web.Areas.GiaoVien.Controllers
{
    [Area("GiaoVien")]
    [Authorize(Roles = "GiaoVien")]
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

            var schedules = await _classScheduleService.GetMyTeachingScheduleAsync(applicationUserId);
            return View(schedules);
        }
    }
}