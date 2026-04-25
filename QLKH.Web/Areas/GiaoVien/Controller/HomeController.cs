using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QLKH.Application.Interfaces.Services;
using QLKH.Domain.Enums;
using QLKH.Infrastructure.Identity;
using QLKH.Web.Areas.GiaoVien.Models;

namespace QLKH.Web.Areas.GiaoVien.Controllers
{
    [Area("GiaoVien")]
    [Authorize(Roles = "Admin,GiaoVien")]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITeacherService _teacherService;
        private readonly IClassScheduleService _classScheduleService;
        private readonly IClassMaterialService _classMaterialService;
        private readonly IAssignmentService _assignmentService;
        private readonly ISubmissionService _submissionService;
        private readonly IEnrollmentService _enrollmentService;

        public HomeController(
            UserManager<ApplicationUser> userManager,
            ITeacherService teacherService,
            IClassScheduleService classScheduleService,
            IClassMaterialService classMaterialService,
            IAssignmentService assignmentService,
            ISubmissionService submissionService,
            IEnrollmentService enrollmentService)
        {
            _userManager = userManager;
            _teacherService = teacherService;
            _classScheduleService = classScheduleService;
            _classMaterialService = classMaterialService;
            _assignmentService = assignmentService;
            _submissionService = submissionService;
            _enrollmentService = enrollmentService;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return Challenge();
            }

            var applicationUserId = currentUser.Id;
            var today = DateTime.Today;
            var next7Days = today.AddDays(7);

            var teacher = await _teacherService.GetByApplicationUserIdAsync(applicationUserId);

            var myClasses = (await _teacherService.GetMyTeachingClassesAsync(applicationUserId)).ToList();
            var classRoomIds = myClasses.Select(x => x.Id).ToList();

            var schedules = (await _classScheduleService.GetMyTeachingScheduleAsync(applicationUserId)).ToList();
            var materials = (await _classMaterialService.GetMyTeachingMaterialsAsync(applicationUserId)).ToList();
            var assignments = (await _assignmentService.GetMyTeachingAssignmentsAsync(applicationUserId)).ToList();

            var assignmentIds = assignments.Select(x => x.Id).ToList();

            var allEnrollments = (await _enrollmentService.GetAllAsync()).ToList();
            var myEnrollments = allEnrollments
                .Where(x => classRoomIds.Contains(x.ClassRoomId))
                .ToList();

            var allSubmissions = (await _submissionService.GetAllAsync()).ToList();
            var mySubmissions = allSubmissions
                .Where(x => assignmentIds.Contains(x.AssignmentId))
                .ToList();

            var model = new GiaoVienDashboardViewModel
            {
                TeacherFullName = teacher?.FullName ?? currentUser.FullName ?? "Giáo viên",
                TeacherEmail = teacher?.Email ?? currentUser.Email ?? string.Empty,
                TeacherRole = "Giáo viên",
                TeacherCode = teacher?.TeacherCode ?? "Chưa liên kết",
                Specialization = teacher?.Specialization,

                TotalClasses = myClasses.Count,
                ActiveClasses = myClasses.Count(x => x.StartDate.Date <= today && x.EndDate.Date >= today),
                EndedClasses = myClasses.Count(x => x.EndDate.Date < today),

                TotalStudents = myEnrollments
                    .Where(x => x.Status == EnrollmentStatus.Confirmed)
                    .Select(x => x.StudentId)
                    .Distinct()
                    .Count(),

                PendingEnrollments = myEnrollments.Count(x => x.Status == EnrollmentStatus.Pending),

                TotalSchedules = schedules.Count,
                TodaySchedules = schedules.Count(x => x.StudyDate.Date == today),
                UpcomingSchedulesCount = schedules.Count(x => x.StudyDate.Date >= today && x.StudyDate.Date <= next7Days),

                TotalMaterials = materials.Count,
                TotalAssignments = assignments.Count,

                TotalSubmissions = mySubmissions.Count,
                GradedSubmissions = mySubmissions.Count(x => x.Score.HasValue),
                UngradedSubmissions = mySubmissions.Count(x => !x.Score.HasValue),

                UpcomingSchedules = schedules
                    .Where(x => x.StudyDate.Date >= today)
                    .OrderBy(x => x.StudyDate)
                    .ThenBy(x => x.StartTime)
                    .Take(5)
                    .Select(x => new GiaoVienScheduleItemViewModel
                    {
                        ClassName = x.ClassRoom?.ClassName ?? string.Empty,
                        LessonTitle = string.IsNullOrWhiteSpace(x.LessonTitle) ? "Buổi học" : x.LessonTitle,
                        StudyDate = x.StudyDate,
                        StartTime = x.StartTime,
                        EndTime = x.EndTime,
                        RoomName = x.RoomName
                    })
                    .ToList(),

                LatestAssignments = assignments
                    .OrderByDescending(x => x.CreatedAt)
                    .Take(5)
                    .Select(x => new GiaoVienAssignmentItemViewModel
                    {
                        AssignmentId = x.Id,
                        ClassName = x.ClassRoom?.ClassName ?? string.Empty,
                        Title = x.Title,
                        DueDate = x.DueDate,
                        CreatedAt = x.CreatedAt
                    })
                    .ToList(),

                RecentNotes = new List<string>
                {
                    "Theo dõi lịch dạy trong 7 ngày tới để chuẩn bị tài liệu và bài tập.",
                    "Ưu tiên chấm các bài nộp chưa có điểm để học viên theo dõi tiến độ học tập.",
                    "Cập nhật tài liệu học tập thường xuyên cho các lớp đang phụ trách."
                }
            };

            return View(model);
        }
    }
}