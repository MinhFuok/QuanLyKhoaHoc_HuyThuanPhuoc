using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public async Task<IActionResult> Index(int? classRoomId)
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(applicationUserId))
            {
                return Challenge();
            }

            var allAssignments = (await _assignmentService.GetMyLearningAssignmentsAsync(applicationUserId))
                .ToList();

            var classRoomOptions = allAssignments
                .Where(x => x.ClassRoom != null)
                .GroupBy(x => x.ClassRoomId)
                .Select(g =>
                {
                    var first = g.First();

                    return new SelectListItem
                    {
                        Value = first.ClassRoomId.ToString(),
                        Text = $"{first.ClassRoom!.ClassCode} - {first.ClassRoom.ClassName}"
                    };
                })
                .OrderBy(x => x.Text)
                .ToList();

            var assignments = allAssignments;

            if (classRoomId.HasValue && classRoomId.Value > 0)
            {
                assignments = assignments
                    .Where(x => x.ClassRoomId == classRoomId.Value)
                    .ToList();
            }

            var submissions = await _submissionService.GetMySubmissionsAsync(applicationUserId);

            ViewBag.SubmittedAssignmentIds = submissions
                .Select(x => x.AssignmentId)
                .Distinct()
                .ToList();

            ViewBag.ClassRoomOptions = classRoomOptions;
            ViewBag.SelectedClassRoomId = classRoomId;

            return View(assignments);
        }
    }
}