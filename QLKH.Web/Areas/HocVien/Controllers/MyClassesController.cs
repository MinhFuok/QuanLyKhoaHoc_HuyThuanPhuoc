using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using QLKH.Application.Interfaces.Services;

namespace QLKH.Web.Areas.HocVien.Controllers
{
    [Area("HocVien")]
    [Authorize(Roles = "HocVien")]
    public class MyClassesController : Controller
    {
        private readonly IStudentService _studentService;

        public MyClassesController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        public async Task<IActionResult> Index()
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(applicationUserId))
            {
                return Challenge();
            }

            var classes = await _studentService.GetMyClassesAsync(applicationUserId);
            return View(classes);
        }
    }
}