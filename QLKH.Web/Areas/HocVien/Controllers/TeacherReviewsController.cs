using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKH.Application.Interfaces.Services;
using System.Security.Claims;

namespace QLKH.Web.Areas.HocVien.Controllers
{
    [Area("HocVien")]
    [Authorize(Roles = "HocVien")]
    public class TeacherReviewsController : Controller
    {
        private readonly ITeacherReviewService _teacherReviewService;
        private readonly IStudentService _studentService;
        private readonly IClassRoomService _classRoomService;

        public TeacherReviewsController(
            ITeacherReviewService teacherReviewService,
            IStudentService studentService,
            IClassRoomService classRoomService)
        {
            _teacherReviewService = teacherReviewService;
            _studentService = studentService;
            _classRoomService = classRoomService;
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int classRoomId)
        {
            var classRoom = await _classRoomService.GetByIdAsync(classRoomId);
            if (classRoom == null)
            {
                return NotFound();
            }

            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(applicationUserId))
            {
                return Challenge();
            }

            var myClasses = await _studentService.GetMyClassesAsync(applicationUserId);
            var isMyClass = myClasses.Any(x => x.Id == classRoomId);

            if (!isMyClass)
            {
                return Forbid();
            }

            if (classRoom.EndDate.Date > DateTime.Today)
            {
                TempData["ErrorMessage"] = "Bạn chỉ được đánh giá giáo viên sau khi kết thúc môn/lớp.";
                return RedirectToAction("Index", "MyLearningProgress", new { area = "HocVien" });
            }

            var review = await _teacherReviewService.GetMyReviewForClassAsync(applicationUserId, classRoomId);

            ViewBag.ClassRoom = classRoom;
            ViewBag.ExistingReview = review;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int classRoomId, int rating, string? comment)
        {
            var classRoom = await _classRoomService.GetByIdAsync(classRoomId);
            if (classRoom == null)
            {
                return NotFound();
            }

            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(applicationUserId))
            {
                return Challenge();
            }

            var myClasses = await _studentService.GetMyClassesAsync(applicationUserId);
            var isMyClass = myClasses.Any(x => x.Id == classRoomId);

            if (!isMyClass)
            {
                return Forbid();
            }

            if (classRoom.EndDate.Date > DateTime.Today)
            {
                TempData["ErrorMessage"] = "Bạn chỉ được đánh giá giáo viên sau khi kết thúc môn/lớp.";
                return RedirectToAction("Index", "MyLearningProgress", new { area = "HocVien" });
            }

            var result = await _teacherReviewService.SubmitReviewAsync(applicationUserId, classRoomId, rating, comment);

            if (!result)
            {
                ViewBag.ClassRoom = classRoom;
                ViewBag.ExistingReview = await _teacherReviewService.GetMyReviewForClassAsync(applicationUserId, classRoomId);
                ViewBag.ErrorMessage = "Gửi đánh giá thất bại. Vui lòng kiểm tra lại dữ liệu.";
                return View();
            }

            TempData["SuccessMessage"] = "Đánh giá giáo viên thành công.";
            return RedirectToAction("Edit", new { classRoomId });
        }
    }
}