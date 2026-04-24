using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QLKH.Application.Interfaces.Services;
using QLKH.Domain.Entities;
using QLKH.Web.Areas.HocVu.Models;

namespace QLKH.Web.Areas.HocVu.Controllers
{
    [Area("HocVu")]
    [Authorize(Roles = "HocVu,Admin")]
    public class StudentCertificateController : Controller
    {
        private readonly IStudentCertificateService _studentCertificateService;
        private readonly IStudentService _studentService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public StudentCertificateController(
            IStudentCertificateService studentCertificateService,
            IStudentService studentService,
            IWebHostEnvironment webHostEnvironment)
        {
            _studentCertificateService = studentCertificateService;
            _studentService = studentService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var certificates = await _studentCertificateService.GetAllAsync();
            return View(certificates);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new StudentCertificateViewModel();
            await LoadStudentOptions(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentCertificateViewModel model)
        {
            await LoadStudentOptions(model);

            if (!ModelState.IsValid)
            {
                TempData.Remove("ErrorMessage");
                TempData.Remove("SuccessMessage");
                return View(model);
            }

            string? savedFilePath = null;

            if (model.EvidenceFile != null && model.EvidenceFile.Length > 0)
            {
                var extension = Path.GetExtension(model.EvidenceFile.FileName).ToLowerInvariant();
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("EvidenceFile", "Chỉ chấp nhận file .jpg, .jpeg, .png, .pdf");
                    TempData.Remove("ErrorMessage");
                    TempData.Remove("SuccessMessage");
                    return View(model);
                }

                var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "student-certificates");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var fileName = $"certificate_{Guid.NewGuid():N}{extension}";
                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.EvidenceFile.CopyToAsync(stream);
                }

                savedFilePath = $"/uploads/student-certificates/{fileName}";
            }

            var entity = new StudentCertificate
            {
                StudentId = model.StudentId,
                CertificateName = model.CertificateName,
                CertificateCode = model.CertificateCode,
                IssuedDate = model.IssuedDate,
                IssuedBy = model.IssuedBy,
                Note = model.Note,
                EvidenceFilePath = savedFilePath,
                IsApproved = model.IsApproved,
                CreatedAt = DateTime.Now
            };

            try
            {
                await _studentCertificateService.CreateAsync(entity);
                TempData["SuccessMessage"] = "Thêm chứng chỉ cho học viên thành công.";
                TempData.Remove("ErrorMessage");
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["ErrorMessage"] = "Thêm chứng chỉ thất bại. Vui lòng kiểm tra lại thông tin nhập.";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _studentCertificateService.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            var model = new StudentCertificateViewModel
            {
                Id = entity.Id,
                StudentId = entity.StudentId,
                CertificateName = entity.CertificateName,
                CertificateCode = entity.CertificateCode,
                IssuedDate = entity.IssuedDate,
                IssuedBy = entity.IssuedBy,
                Note = entity.Note,
                ExistingEvidenceFilePath = entity.EvidenceFilePath,
                IsApproved = entity.IsApproved
            };

            await LoadStudentOptions(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentCertificateViewModel model)
        {
            await LoadStudentOptions(model);

            if (!ModelState.IsValid)
            {
                TempData.Remove("ErrorMessage");
                TempData.Remove("SuccessMessage");
                return View(model);
            }

            var entity = await _studentCertificateService.GetByIdAsync(model.Id);
            if (entity == null)
            {
                return NotFound();
            }

            entity.StudentId = model.StudentId;
            entity.CertificateName = model.CertificateName;
            entity.CertificateCode = model.CertificateCode;
            entity.IssuedDate = model.IssuedDate;
            entity.IssuedBy = model.IssuedBy;
            entity.Note = model.Note;
            entity.IsApproved = model.IsApproved;

            if (model.EvidenceFile != null && model.EvidenceFile.Length > 0)
            {
                var extension = Path.GetExtension(model.EvidenceFile.FileName).ToLowerInvariant();
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("EvidenceFile", "Chỉ chấp nhận file .jpg, .jpeg, .png, .pdf");
                    TempData.Remove("ErrorMessage");
                    TempData.Remove("SuccessMessage");
                    return View(model);
                }

                var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "student-certificates");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var fileName = $"certificate_{Guid.NewGuid():N}{extension}";
                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.EvidenceFile.CopyToAsync(stream);
                }

                entity.EvidenceFilePath = $"/uploads/student-certificates/{fileName}";
            }

            try
            {
                await _studentCertificateService.UpdateAsync(entity);
                TempData["SuccessMessage"] = "Cập nhật chứng chỉ thành công.";
                TempData.Remove("ErrorMessage");
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["ErrorMessage"] = "Cập nhật chứng chỉ thất bại. Vui lòng thử lại.";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _studentCertificateService.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            return View(entity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _studentCertificateService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Xóa chứng chỉ thành công.";
                TempData.Remove("ErrorMessage");
            }
            catch
            {
                TempData["ErrorMessage"] = "Xóa chứng chỉ thất bại.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadStudentOptions(StudentCertificateViewModel model)
        {
            var students = await _studentService.GetAllAsync();

            model.StudentOptions = students
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.FullName} - {s.Email}"
                })
                .ToList();
        }
    }
}