using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLKH.Application.Interfaces.Services;
using QLKH.Infrastructure.Identity;
using QLKH.Web.Areas.Admin.Models;

namespace QLKH.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStudentService _studentService;
        private readonly ITeacherService _teacherService;
        private readonly ICourseService _courseService;
        private readonly IClassRoomService _classRoomService;
        private readonly IDepartmentService _departmentService;
        private readonly ISystemSettingService _systemSettingService;
        private readonly IHomeBannerSlideService _homeBannerSlideService;
        private readonly IAdminAuditLogService _adminAuditLogService;

        public HomeController(
            UserManager<ApplicationUser> userManager,
            IStudentService studentService,
            ITeacherService teacherService,
            ICourseService courseService,
            IClassRoomService classRoomService,
            IDepartmentService departmentService,
            ISystemSettingService systemSettingService,
            IHomeBannerSlideService homeBannerSlideService,
            IAdminAuditLogService adminAuditLogService)
        {
            _userManager = userManager;
            _studentService = studentService;
            _teacherService = teacherService;
            _courseService = courseService;
            _classRoomService = classRoomService;
            _departmentService = departmentService;
            _systemSettingService = systemSettingService;
            _homeBannerSlideService = homeBannerSlideService;
            _adminAuditLogService = adminAuditLogService;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentRoles = currentUser != null
                ? await _userManager.GetRolesAsync(currentUser)
                : new List<string>();

            var users = await _userManager.Users.ToListAsync();

            var students = await _studentService.GetAllAsync();
            var teachers = await _teacherService.GetAllAsync();
            var courses = await _courseService.GetAllAsync();
            var classRooms = await _classRoomService.GetAllAsync();
            var departments = await _departmentService.GetAllAsync();

            var setting = await _systemSettingService.GetAsync();
            var activeBanners = await _homeBannerSlideService.GetActiveSlidesAsync();

            var allLogs = await _adminAuditLogService.GetAllAsync();

            var now = DateTimeOffset.UtcNow;

            var model = new AdminDashboardViewModel
            {
                AdminFullName = currentUser?.FullName ?? "Admin",
                AdminEmail = currentUser?.Email ?? "",
                AdminRole = currentRoles.FirstOrDefault() ?? "Admin",

                TotalUsers = users.Count,
                ActiveUsers = users.Count(x => !x.LockoutEnd.HasValue || x.LockoutEnd <= now),
                LockedUsers = users.Count(x => x.LockoutEnd.HasValue && x.LockoutEnd > now),

                TotalStudents = students.Count(),
                LinkedStudents = students.Count(x => !string.IsNullOrWhiteSpace(x.ApplicationUserId)),

                TotalTeachers = teachers.Count(),
                LinkedTeachers = teachers.Count(x => !string.IsNullOrWhiteSpace(x.ApplicationUserId)),

                TotalCourses = courses.Count(),
                TotalClassRooms = classRooms.Count(),
                TotalDepartments = departments.Count(),

                IsWebsiteEnabled = setting?.IsWebsiteEnabled ?? true,
                EnableEmail = setting?.EnableEmail ?? false,
                ActiveBannerCount = activeBanners.Count(),
                SiteName = setting?.SiteName ?? "QLKH",
                MaintenanceMessage = setting?.MaintenanceMessage ?? "",

                RecentActivities = allLogs
                    .OrderByDescending(x => x.CreatedAt)
                    .Take(8)
                    .Select(x => new AdminDashboardActivityItemViewModel
                    {
                        CreatedAt = x.CreatedAt,
                        ActorEmail = x.ActorEmail ?? "",
                        ActionName = x.ActionName ?? "",
                        TargetType = x.TargetType ?? "",
                        TargetDisplay = x.TargetDisplay ?? "",
                        Note = x.Note ?? ""
                    })
                    .ToList()
            };

            return View(model);
        }
    }
}