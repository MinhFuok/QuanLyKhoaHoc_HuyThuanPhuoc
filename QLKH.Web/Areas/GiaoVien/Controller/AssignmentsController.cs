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
        public async Task<IActionResult> Create(int? classRoomId)
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(applicationUserId))
            {
                return Challenge();
            }

            var myClasses = (await _teacherService.GetMyTeachingClassesAsync(applicationUserId)).ToList();
            ViewBag.MyClasses = myClasses;

            if (classRoomId.HasValue && myClasses.Any(x => x.Id == classRoomId.Value))
            {
                ViewBag.SelectedClassRoomId = classRoomId.Value;
            }

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

            if (dueDate == default)
            {
                ViewBag.ErrorMessage = "Hạn nộp là bắt buộc.";
                return View();
            }

            var assignment = new Assignment
            {
                ClassRoomId = classRoomId,
                Title = title.Trim(),
                Description = description,
                DueDate = dueDate,
                CreatedAt = DateTime.Now
            };

            await _assignmentService.AddAsync(assignment);

            TempData["SuccessMessage"] = "Tạo bài tập thành công.";
            return RedirectToAction("Create");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(applicationUserId))
            {
                return Challenge();
            }

            var assignment = await _assignmentService.GetByIdAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }

            var myClasses = (await _teacherService.GetMyTeachingClassesAsync(applicationUserId)).ToList();
            var isMyAssignment = myClasses.Any(x => x.Id == assignment.ClassRoomId);

            if (!isMyAssignment)
            {
                return Forbid();
            }

            ViewBag.MyClasses = myClasses;

            return View(assignment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int classRoomId, string title, string? description, DateTime dueDate)
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(applicationUserId))
            {
                return Challenge();
            }

            var assignment = await _assignmentService.GetByIdAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }

            var myClasses = (await _teacherService.GetMyTeachingClassesAsync(applicationUserId)).ToList();
            ViewBag.MyClasses = myClasses;

            var isOldClassMine = myClasses.Any(x => x.Id == assignment.ClassRoomId);
            if (!isOldClassMine)
            {
                return Forbid();
            }

            var isNewClassMine = myClasses.Any(x => x.Id == classRoomId);
            if (!isNewClassMine)
            {
                ViewBag.ErrorMessage = "Bạn chỉ được chuyển bài tập sang lớp mình dạy.";
                return View(assignment);
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                ViewBag.ErrorMessage = "Tiêu đề bài tập là bắt buộc.";
                return View(assignment);
            }

            if (dueDate == default)
            {
                ViewBag.ErrorMessage = "Hạn nộp là bắt buộc.";
                return View(assignment);
            }

            assignment.ClassRoomId = classRoomId;
            assignment.Title = title.Trim();
            assignment.Description = description;
            assignment.DueDate = dueDate;

            await _assignmentService.UpdateAsync(assignment);

            TempData["SuccessMessage"] = "Cập nhật bài tập thành công.";
            return RedirectToAction("Index", "MyAssignments", new { area = "GiaoVien" });
        }
    }
}