using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public async Task<IActionResult> Index(int? classRoomId)
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(applicationUserId))
            {
                return Challenge();
            }

            var myClasses = (await _teacherService.GetMyTeachingClassesAsync(applicationUserId))
                .OrderBy(x => x.ClassCode)
                .ToList();

            ViewBag.ClassRoomOptions = myClasses
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"{x.ClassCode} - {x.ClassName}"
                })
                .ToList();

            if (!myClasses.Any())
            {
                ViewBag.SelectedClassRoomId = null;
                return View(null);
            }

            var selectedClassRoomId = classRoomId.HasValue && myClasses.Any(x => x.Id == classRoomId.Value)
                ? classRoomId.Value
                : myClasses.First().Id;

            ViewBag.SelectedClassRoomId = selectedClassRoomId;

            var model = await _submissionService.GetTeacherGradeOverviewByClassAsync(selectedClassRoomId);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }
    }
}