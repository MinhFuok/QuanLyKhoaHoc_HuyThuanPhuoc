using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QLKH.Application.Interfaces.Services;
using QLKH.Domain.Entities;
using QLKH.Web.Areas.Admin.Models;

namespace QLKH.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        public async Task<IActionResult> Index(string? keyword)
        {
            var departments = await _departmentService.GetAllAsync();

            ViewBag.Keyword = keyword;

            return View(departments);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadParentDepartmentsAsync();

            var model = new DepartmentCreateViewModel
            {
                DepartmentCode = await GenerateNextDepartmentCodeAsync()
            };

            return View(model);
        }
        private async Task<string> GenerateNextDepartmentCodeAsync()
        {
            var departments = await _departmentService.GetAllAsync();

            var lastCode = departments
                .Where(x => !string.IsNullOrWhiteSpace(x.DepartmentCode) && x.DepartmentCode.StartsWith("KM"))
                .OrderByDescending(x => x.DepartmentCode)
                .Select(x => x.DepartmentCode)
                .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(lastCode))
            {
                return "KM001";
            }

            var numberPart = lastCode.Substring(2);

            if (!int.TryParse(numberPart, out int number))
            {
                return "KM001";
            }

            return $"KM{(number + 1):D3}";
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentCreateViewModel model)
        {
            await LoadParentDepartmentsAsync();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var department = new Department
            {
                DepartmentCode = await GenerateNextDepartmentCodeAsync(),
                DepartmentName = model.DepartmentName.Trim(),
                Description = model.Description,
                ParentDepartmentId = model.ParentDepartmentId,
                IsActive = model.IsActive
            };

            await _departmentService.AddAsync(department);

            TempData["SuccessMessage"] = "Thêm khoa/môn thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var department = await _departmentService.GetByIdAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            var model = new DepartmentEditViewModel
            {
                Id = department.Id,
                DepartmentCode = department.DepartmentCode,
                DepartmentName = department.DepartmentName,
                Description = department.Description,
                ParentDepartmentId = department.ParentDepartmentId,
                IsActive = department.IsActive
            };

            await LoadParentDepartmentsAsync(department.Id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DepartmentEditViewModel model)
        {
            await LoadParentDepartmentsAsync(model.Id);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var department = await _departmentService.GetByIdAsync(model.Id);
            if (department == null)
            {
                return NotFound();
            }

            department.DepartmentCode = model.DepartmentCode.Trim();
            department.DepartmentName = model.DepartmentName.Trim();
            department.Description = model.Description;
            department.ParentDepartmentId = model.ParentDepartmentId;
            department.IsActive = model.IsActive;

            await _departmentService.UpdateAsync(department);

            TempData["SuccessMessage"] = "Cập nhật khoa/môn thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _departmentService.GetByIdAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var departments = await _departmentService.GetAllAsync();
            var department = departments.FirstOrDefault(d => d.Id == id);

            if (department == null)
            {
                return NotFound();
            }

            var hasChildren = departments.Any(d => d.ParentDepartmentId == id);

            if (hasChildren)
            {
                TempData["ErrorMessage"] = "Không thể xóa khoa này vì đang có ngành con.";
                return RedirectToAction(nameof(Index));
            }

            await _departmentService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Xóa khoa/môn thành công.";

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadParentDepartmentsAsync(int? excludeId = null)
        {
            var parents = await _departmentService.GetParentDepartmentsAsync();

            if (excludeId.HasValue)
            {
                parents = parents.Where(d => d.Id != excludeId.Value);
            }

            ViewBag.ParentDepartments = parents.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = $"{d.DepartmentCode} - {d.DepartmentName}"
            }).ToList();
        }
    }
}