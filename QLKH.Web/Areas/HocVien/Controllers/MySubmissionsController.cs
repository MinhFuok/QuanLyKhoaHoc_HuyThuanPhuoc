using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKH.Application.Interfaces.Services;
using System.Security.Claims;

namespace QLKH.Web.Areas.HocVien.Controllers
{
    [Area("HocVien")]
    [Authorize(Roles = "HocVien")]
    public class MySubmissionsController : Controller
    {
        private readonly ISubmissionService _submissionService;
        private readonly IAssignmentService _assignmentService;
        private readonly IStudentService _studentService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MySubmissionsController(
            ISubmissionService submissionService,
            IAssignmentService assignmentService,
            IStudentService studentService,
            IWebHostEnvironment webHostEnvironment)
        {
            _submissionService = submissionService;
            _assignmentService = assignmentService;
            _studentService = studentService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Submit(int assignmentId)
        {
            var assignment = await _assignmentService.GetByIdAsync(assignmentId);
            if (assignment == null)
            {
                return NotFound();
            }

            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(applicationUserId))
            {
                return Challenge();
            }

            var myClasses = await _studentService.GetMyClassesAsync(applicationUserId);
            var isMyClass = myClasses.Any(x => x.Id == assignment.ClassRoomId);

            if (!isMyClass)
            {
                return Forbid();
            }

            var mySubmissions = await _submissionService.GetMySubmissionsAsync(applicationUserId);

            var existingSubmission = mySubmissions
                .FirstOrDefault(x => x.AssignmentId == assignmentId);

            ViewBag.Assignment = assignment;
            ViewBag.ExistingSubmission = existingSubmission;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(
            int assignmentId,
            string? submissionText,
            IFormFile? submissionFile)
        {
            var assignment = await _assignmentService.GetByIdAsync(assignmentId);
            if (assignment == null)
            {
                return NotFound();
            }

            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(applicationUserId))
            {
                return Challenge();
            }

            var myClasses = await _studentService.GetMyClassesAsync(applicationUserId);
            var isMyClass = myClasses.Any(x => x.Id == assignment.ClassRoomId);

            if (!isMyClass)
            {
                return Forbid();
            }

            var mySubmissions = await _submissionService.GetMySubmissionsAsync(applicationUserId);

            var existingSubmission = mySubmissions
                .FirstOrDefault(x => x.AssignmentId == assignmentId);

            string? filePath = existingSubmission?.FilePath;

            if (submissionFile != null && submissionFile.Length > 0)
            {
                var uploadsRoot = Path.Combine(_webHostEnvironment.WebRootPath, "submissions");
                Directory.CreateDirectory(uploadsRoot);

                var extension = Path.GetExtension(submissionFile.FileName);
                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var physicalPath = Path.Combine(uploadsRoot, uniqueFileName);

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    await submissionFile.CopyToAsync(stream);
                }

                filePath = $"/submissions/{uniqueFileName}";
            }

            var result = await _submissionService.SubmitAsync(
                applicationUserId,
                assignmentId,
                submissionText,
                filePath);

            if (!result)
            {
                ViewBag.Assignment = assignment;
                ViewBag.ExistingSubmission = existingSubmission;
                ViewBag.ErrorMessage = "Nộp bài thất bại. Vui lòng nhập nội dung bài nộp hoặc chọn file.";
                return View();
            }

            TempData["SuccessMessage"] = existingSubmission == null
                ? "Nộp bài thành công."
                : "Cập nhật bài nộp thành công.";

            return RedirectToAction("Submit", new { assignmentId });
        }
    }
}