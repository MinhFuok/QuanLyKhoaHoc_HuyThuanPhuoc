using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKH.Application.Interfaces.Services;
using System.Security.Claims;

namespace QLKH.Web.Areas.HocVien.Controllers
{
    [Area("HocVien")]
    [Authorize(Roles = "HocVien")]
    public class MyLearningProgressController : Controller
    {
        private readonly ISubmissionService _submissionService;

        public MyLearningProgressController(ISubmissionService submissionService)
        {
            _submissionService = submissionService;
        }

        public async Task<IActionResult> Index()
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(applicationUserId))
            {
                return Challenge();
            }

            var model = await _submissionService.GetStudentLearningProgressOverviewAsync(applicationUserId);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }
    }
}