using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKH.Application.Interfaces.Services;
using System.Security.Claims;

namespace QLKH.Web.Areas.HocVien.Controllers
{
    [Area("HocVien")]
    [Authorize(Roles = "HocVien")]
    public class MySubmissionResultsController : Controller
    {
        private readonly ISubmissionService _submissionService;
        private readonly IAssignmentService _assignmentService;
        private readonly IStudentService _studentService;

        public MySubmissionResultsController(
            ISubmissionService submissionService,
            IAssignmentService assignmentService,
            IStudentService studentService)
        {
            _submissionService = submissionService;
            _assignmentService = assignmentService;
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<IActionResult> Details(int assignmentId)
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

            var submission = await _submissionService
                .GetByAssignmentAndCurrentStudentAsync(applicationUserId, assignmentId);

            ViewBag.Assignment = assignment;
            return View(submission);
        }
    }
}