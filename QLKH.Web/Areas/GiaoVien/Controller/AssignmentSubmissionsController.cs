using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKH.Application.Interfaces.Services;
using System.Security.Claims;

namespace QLKH.Web.Areas.GiaoVien.Controllers
{
    [Area("GiaoVien")]
    [Authorize(Roles = "GiaoVien")]
    public class AssignmentSubmissionsController : Controller
    {
        private readonly ISubmissionService _submissionService;
        private readonly IAssignmentService _assignmentService;
        private readonly ITeacherService _teacherService;

        public AssignmentSubmissionsController(
            ISubmissionService submissionService,
            IAssignmentService assignmentService,
            ITeacherService teacherService)
        {
            _submissionService = submissionService;
            _assignmentService = assignmentService;
            _teacherService = teacherService;
        }

        public async Task<IActionResult> Index(int assignmentId)
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

            var myClasses = await _teacherService.GetMyTeachingClassesAsync(applicationUserId);
            var isMyClass = myClasses.Any(x => x.Id == assignment.ClassRoomId);

            if (!isMyClass)
            {
                return Forbid();
            }

            var submissions = await _submissionService.GetByAssignmentIdAsync(assignmentId);
            ViewBag.Assignment = assignment;

            return View(submissions);
        }
    }
}