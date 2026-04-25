using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKH.Application.Interfaces.Services;
using System.Security.Claims;

namespace QLKH.Web.Areas.HocVien.Controllers
{
    [Area("HocVien")]
    [Authorize(Roles = "HocVien")]
    public class MyAssignmentsController : Controller
    {
        private readonly IAssignmentService _assignmentService;
        private readonly ISubmissionService _submissionService;

        public MyAssignmentsController(
            IAssignmentService assignmentService,
            ISubmissionService submissionService)
        {
            _assignmentService = assignmentService;
            _submissionService = submissionService;
        }

        public async Task<IActionResult> Index()
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(applicationUserId))
            {
                return Challenge();
            }

            var assignments = await _assignmentService.GetMyLearningAssignmentsAsync(applicationUserId);

            var submissions = await _submissionService.GetMySubmissionsAsync(applicationUserId);

            ViewBag.SubmittedAssignmentIds = submissions
                .Select(x => x.AssignmentId)
                .Distinct()
                .ToList();

            return View(assignments);
        }
    }
}