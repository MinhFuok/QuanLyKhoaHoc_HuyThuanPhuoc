using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKH.Application.Interfaces.Services;
using QLKH.Domain.Entities;
using System.Security.Claims;

namespace QLKH.Web.Areas.GiaoVien.Controllers
{
    [Area("GiaoVien")]
    [Authorize(Roles = "GiaoVien")]
    public class AssignmentsController : Controller
    {
        private readonly ITeacherService _teacherService;
        private readonly IAssignmentService _assignmentService;

        public AssignmentsController(
            ITeacherService teacherService,
            IAssignmentService assignmentService)
        {
            _teacherService = teacherService;
            _assignmentService = assignmentService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(applicationUserId))
            {
                return Challenge();
            }

            var myClasses = await _teacherService.GetMyTeachingClassesAsync(applicationUserId);
            ViewBag.MyClasses = myClasses;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int classRoomId, string title, string? description, DateTime dueDate)
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(applicationUserId))
            {
                return Challenge();
            }

            var myClasses = await _teacherService.GetMyTeachingClassesAsync(applicationUserId);
            ViewBag.MyClasses = myClasses;

            var isMyClass = myClasses.Any(x => x.Id == classRoomId);
            if (!isMyClass)
            {
                ViewBag.ErrorMessage = "Bạn chỉ được tạo bài tập cho lớp mình dạy.";
                return View();
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                ViewBag.ErrorMessage = "Tiêu đề bài tập là bắt buộc.";
                return View();
            }

            var assignment = new Assignment
            {
                ClassRoomId = classRoomId,
                Title = title,
                Description = description,
                DueDate = dueDate,
                CreatedAt = DateTime.Now
            };

            await _assignmentService.AddAsync(assignment);

            TempData["SuccessMessage"] = "Tạo bài tập thành công.";
            return RedirectToAction("Create");
        }
    }
}