using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKH.Application.Interfaces.Services;
using System.Security.Claims;

namespace QLKH.Web.Areas.HocVien.Controllers
{
    [Area("HocVien")]
    [Authorize(Roles = "HocVien")]
    public class MyMaterialsController : Controller
    {
        private readonly IClassMaterialService _classMaterialService;

        public MyMaterialsController(IClassMaterialService classMaterialService)
        {
            _classMaterialService = classMaterialService;
        }

        public async Task<IActionResult> Index()
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(applicationUserId))
            {
                return Challenge();
            }

            var materials = await _classMaterialService.GetMyLearningMaterialsAsync(applicationUserId);
            return View(materials);
        }
    }
}