using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKH.Application.Interfaces.Services;
using QLKH.Domain.Entities;
using QLKH.Web.Areas.Admin.Models;

namespace QLKH.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeBannerSlideController : Controller
    {
        private readonly IHomeBannerSlideService _slideService;
        private readonly IWebHostEnvironment _environment;

        public HomeBannerSlideController(
            IHomeBannerSlideService slideService,
            IWebHostEnvironment environment)
        {
            _slideService = slideService;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            var slides = await _slideService.GetAllAsync();

            var model = slides.Select(x => new HomeBannerSlideViewModel
            {
                Id = x.Id,
                Title = x.Title,
                ExistingImageUrl = x.ImageUrl,
                AltText = x.AltText,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive
            }).ToList();

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new HomeBannerSlideViewModel
            {
                IsActive = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HomeBannerSlideViewModel model)
        {
            if (model.ImageFile == null || model.ImageFile.Length == 0)
            {
                ModelState.AddModelError("ImageFile", "Vui lòng chọn ảnh banner.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var imageUrl = await SaveImageAsync(model.ImageFile!);
            var nextOrder = await _slideService.GetNextDisplayOrderAsync();

            var slide = new HomeBannerSlide
            {
                Title = model.Title,
                ImageUrl = imageUrl,
                AltText = model.AltText,
                DisplayOrder = nextOrder,
                IsActive = model.IsActive
            };

            await _slideService.AddAsync(slide);
            TempData["SuccessMessage"] = "Thêm slide banner thành công.";
            return RedirectToSystemSettings();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var slide = await _slideService.GetByIdAsync(id);
            if (slide == null)
            {
                return NotFound();
            }

            var model = new HomeBannerSlideViewModel
            {
                Id = slide.Id,
                Title = slide.Title,
                ExistingImageUrl = slide.ImageUrl,
                AltText = slide.AltText,
                DisplayOrder = slide.DisplayOrder,
                IsActive = slide.IsActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HomeBannerSlideViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var slide = await _slideService.GetByIdAsync(model.Id);
            if (slide == null)
            {
                return NotFound();
            }

            slide.Title = model.Title;
            slide.AltText = model.AltText;
            slide.IsActive = model.IsActive;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var newImageUrl = await SaveImageAsync(model.ImageFile);
                slide.ImageUrl = newImageUrl;
            }

            await _slideService.UpdateAsync(slide);
            TempData["SuccessMessage"] = "Cập nhật slide banner thành công.";
            return RedirectToSystemSettings();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var slide = await _slideService.GetByIdAsync(id);
            if (slide == null)
            {
                return NotFound();
            }

            var model = new HomeBannerSlideViewModel
            {
                Id = slide.Id,
                Title = slide.Title,
                ExistingImageUrl = slide.ImageUrl,
                AltText = slide.AltText,
                DisplayOrder = slide.DisplayOrder,
                IsActive = slide.IsActive
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _slideService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Xóa slide banner thành công.";
            return RedirectToSystemSettings();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveUp(int id)
        {
            await _slideService.MoveUpAsync(id);
            TempData["SuccessMessage"] = "Đã thay đổi thứ tự slide.";
            return RedirectToSystemSettings();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveDown(int id)
        {
            await _slideService.MoveDownAsync(id);
            TempData["SuccessMessage"] = "Đã thay đổi thứ tự slide.";
            return RedirectToSystemSettings();
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "banners");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var extension = Path.GetExtension(imageFile.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"/uploads/banners/{fileName}";
        }

        private IActionResult RedirectToSystemSettings()
        {
            return RedirectToAction("Index", "SystemSettings", new { area = "Admin" });
        }
    }
}