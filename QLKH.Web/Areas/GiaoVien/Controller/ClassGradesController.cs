using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKH.Application.Interfaces.Services;
using System.Security.Claims;

namespace QLKH.Web.Areas.GiaoVien.Controllers
{
    [Area("GiaoVien")]
    [Authorize(Roles = "GiaoVien")]
    public class ClassGradesController : Controller
    {
        private readonly ISubmissionService _submissionService;
        private readonly ITeacherService _teacherService;

        public ClassGradesController(
            ISubmissionService submissionService,
            ITeacherService teacherService)
        {
            _submissionService = submissionService;
            _teacherService = teacherService;
        }

        public async Task<IActionResult> Classes()
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(applicationUserId))
            {
                return Challenge();
            }

            var myClasses = await _teacherService.GetMyTeachingClassesAsync(applicationUserId);
            return View(myClasses);
        }

        public async Task<IActionResult> Index(int classRoomId)
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(applicationUserId))
            {
                return Challenge();
            }

            var myClasses = await _teacherService.GetMyTeachingClassesAsync(applicationUserId);
            var isMyClass = myClasses.Any(x => x.Id == classRoomId);

            if (!isMyClass)
            {
                return Forbid();
            }

            var model = await _submissionService.GetTeacherGradeOverviewByClassAsync(classRoomId);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }
    }
}