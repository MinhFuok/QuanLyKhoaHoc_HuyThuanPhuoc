using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QLKH.Application.Interfaces.Services;
using QLKH.Domain.Enums;
using QLKH.Infrastructure.Identity;
using QLKH.Web.Areas.HocVu.Models;

namespace QLKH.Web.Areas.HocVu.Controllers
{
    [Area("HocVu")]
    [Authorize(Roles = "Admin,HocVu")]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICourseService _courseService;
        private readonly IClassRoomService _classRoomService;
        private readonly IStudentService _studentService;
        private readonly ITeacherService _teacherService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly IStudentCertificateService _studentCertificateService;

        public HomeController(
            UserManager<ApplicationUser> userManager,
            ICourseService courseService,
            IClassRoomService classRoomService,
            IStudentService studentService,
            ITeacherService teacherService,
            IEnrollmentService enrollmentService,
            IStudentCertificateService studentCertificateService)
        {
            _userManager = userManager;
            _courseService = courseService;
            _classRoomService = classRoomService;
            _studentService = studentService;
            _teacherService = teacherService;
            _enrollmentService = enrollmentService;
            _studentCertificateService = studentCertificateService;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var courses = await _courseService.GetAllAsync();
            var classRooms = await _classRoomService.GetAllAsync();
            var students = await _studentService.GetAllAsync();
            var teachers = await _teacherService.GetAllAsync();
            var enrollments = await _enrollmentService.GetAllAsync();
            var certificates = await _studentCertificateService.GetAllAsync();

            var model = new HocVuDashboardViewModel
            {
                HocVuFullName = currentUser?.FullName ?? "Học vụ",
                HocVuEmail = currentUser?.Email ?? string.Empty,
                HocVuRole = "Học vụ",

                TotalCourses = courses.Count(),
                TotalClassRooms = classRooms.Count(),
                TotalStudents = students.Count(),
                TotalTeachers = teachers.Count(),
                TotalEnrollments = enrollments.Count(),
                PendingEnrollments = enrollments.Count(x => x.Status == EnrollmentStatus.Pending),
                ConfirmedEnrollments = enrollments.Count(x => x.Status == EnrollmentStatus.Confirmed),
                CancelledEnrollments = enrollments.Count(x => x.Status == EnrollmentStatus.Cancelled),
                TotalCertificates = certificates.Count(),

                RecentNotes = new List<string>
                {
                    "Theo dõi lớp học sắp khai giảng và tình trạng ghi danh.",
                    "Ưu tiên kiểm tra các ghi danh chờ duyệt để tránh tồn đọng.",
                    "Rà soát học viên đủ điều kiện hoàn thành để xử lý chứng chỉ."
                }
            };

            return View(model);
        }
    }
}