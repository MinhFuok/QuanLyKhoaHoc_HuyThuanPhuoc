using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKH.Application.Interfaces.Services;
using QLKH.Domain.Entities;
using System.Security.Claims;

namespace QLKH.Web.Areas.GiaoVien.Controllers
{
    [Area("GiaoVien")]
    [Authorize(Roles = "GiaoVien")]
    public class ClassMaterialsController : Controller
    {
        private readonly ITeacherService _teacherService;
        private readonly IClassRoomService _classRoomService;
        private readonly IClassMaterialService _classMaterialService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ClassMaterialsController(
            ITeacherService teacherService,
            IClassRoomService classRoomService,
            IClassMaterialService classMaterialService,
            IWebHostEnvironment webHostEnvironment)
        {
            _teacherService = teacherService;
            _classRoomService = classRoomService;
            _classMaterialService = classMaterialService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(applicationUserId))
            {
                return Challenge();
            }

            var myClasses = await _teacherService.GetMyTeachingClassesAsync(applicationUserId);
            ViewBag.MyClasses = myClasses;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int classRoomId, string title, string? description, IFormFile? materialFile)
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(applicationUserId))
            {
                return Challenge();
            }

            var myClasses = await _teacherService.GetMyTeachingClassesAsync(applicationUserId);
            ViewBag.MyClasses = myClasses;

            var isMyClass = myClasses.Any(x => x.Id == classRoomId);
            if (!isMyClass)
            {
                ViewBag.ErrorMessage = "Bạn chỉ được tạo tài liệu cho lớp mình dạy.";
                return View();
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                ViewBag.ErrorMessage = "Tiêu đề tài liệu là bắt buộc.";
                return View();
            }

            if (materialFile == null || materialFile.Length == 0)
            {
                ViewBag.ErrorMessage = "Vui lòng chọn file tài liệu.";
                return View();
            }

            var uploadsRoot = Path.Combine(_webHostEnvironment.WebRootPath, "materials");
            Directory.CreateDirectory(uploadsRoot);

            var extension = Path.GetExtension(materialFile.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var physicalPath = Path.Combine(uploadsRoot, uniqueFileName);

            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await materialFile.CopyToAsync(stream);
            }

            var filePath = $"/materials/{uniqueFileName}";

            var material = new ClassMaterial
            {
                ClassRoomId = classRoomId,
                Title = title,
                Description = description,
                FilePath = filePath,
                CreatedAt = DateTime.Now
            };

            await _classMaterialService.AddAsync(material);

            TempData["SuccessMessage"] = "Tạo tài liệu thành công.";
            return RedirectToAction("Create");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(applicationUserId))
            {
                return Challenge();
            }

            var material = await _classMaterialService.GetByIdAsync(id);
            if (material == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài liệu cần xóa.";
                return RedirectToAction("Index", "MyMaterials", new { area = "GiaoVien" });
            }

            var myClasses = await _teacherService.GetMyTeachingClassesAsync(applicationUserId);
            var isMyClassMaterial = myClasses.Any(x => x.Id == material.ClassRoomId);

            if (!isMyClassMaterial)
            {
                return Forbid();
            }

            if (!string.IsNullOrWhiteSpace(material.FilePath))
            {
                var relativePath = material.FilePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
                var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }
            }

            await _classMaterialService.DeleteAsync(id);

            TempData["SuccessMessage"] = "Xóa tài liệu thành công.";
            return RedirectToAction("Index", "MyMaterials", new { area = "GiaoVien" });
        }
    }
}