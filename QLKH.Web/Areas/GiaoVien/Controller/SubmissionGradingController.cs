using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKH.Application.Interfaces.Services;
using System.Security.Claims;

namespace QLKH.Web.Areas.GiaoVien.Controllers
{
    [Area("GiaoVien")]
    [Authorize(Roles = "GiaoVien")]
    public class SubmissionGradingController : Controller
    {
        private readonly ISubmissionService _submissionService;
        private readonly ITeacherService _teacherService;

        public SubmissionGradingController(
            ISubmissionService submissionService,
            ITeacherService teacherService)
        {
            _submissionService = submissionService;
            _teacherService = teacherService;
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var submission = await _submissionService.GetByIdAsync(id);
            if (submission == null)
            {
                return NotFound();
            }

            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(applicationUserId))
            {
                return Challenge();
            }

            var myClasses = await _teacherService.GetMyTeachingClassesAsync(applicationUserId);
            var classRoomId = submission.Assignment?.ClassRoomId;

            if (!classRoomId.HasValue || !myClasses.Any(x => x.Id == classRoomId.Value))
            {
                return Forbid();
            }

            return View(submission);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, decimal? score, string? feedback)
        {
            var submission = await _submissionService.GetByIdAsync(id);
            if (submission == null)
            {
                return NotFound();
            }

            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(applicationUserId))
            {
                return Challenge();
            }

            var myClasses = await _teacherService.GetMyTeachingClassesAsync(applicationUserId);
            var classRoomId = submission.Assignment?.ClassRoomId;

            if (!classRoomId.HasValue || !myClasses.Any(x => x.Id == classRoomId.Value))
            {
                return Forbid();
            }

            var result = await _submissionService.GradeSubmissionAsync(id, score, feedback);

            if (!result)
            {
                ViewBag.ErrorMessage = "Chấm bài thất bại.";
                return View(submission);
            }

            TempData["SuccessMessage"] = "Chấm bài thành công.";
            return RedirectToAction("Edit", new { id });
        }
    }
}