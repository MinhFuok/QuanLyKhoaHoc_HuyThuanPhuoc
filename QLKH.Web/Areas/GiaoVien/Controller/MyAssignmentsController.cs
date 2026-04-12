using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKH.Application.Interfaces.Services;
using System.Security.Claims;

namespace QLKH.Web.Areas.GiaoVien.Controllers
{
    [Area("GiaoVien")]
    [Authorize(Roles = "GiaoVien")]
    public class MyAssignmentsController : Controller
    {
        private readonly IAssignmentService _assignmentService;

        public MyAssignmentsController(IAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        public async Task<IActionResult> Index()
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(applicationUserId))
            {
                return Challenge();
            }

            var assignments = await _assignmentService.GetMyTeachingAssignmentsAsync(applicationUserId);
            return View(assignments);
        }
    }
}