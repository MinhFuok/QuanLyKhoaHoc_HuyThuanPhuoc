using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public async Task<IActionResult> Index(int? classRoomId)
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(applicationUserId))
            {
                return Challenge();
            }

            var allMaterials = (await _classMaterialService.GetMyLearningMaterialsAsync(applicationUserId))
                .ToList();

            var classRoomOptions = allMaterials
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

            var materials = allMaterials;

            if (classRoomId.HasValue && classRoomId.Value > 0)
            {
                materials = materials
                    .Where(x => x.ClassRoomId == classRoomId.Value)
                    .ToList();
            }

            ViewBag.ClassRoomOptions = classRoomOptions;
            ViewBag.SelectedClassRoomId = classRoomId;

            return View(materials);
        }
    }
}