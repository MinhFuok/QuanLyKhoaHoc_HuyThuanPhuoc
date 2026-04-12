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

        public async Task<IActionResult> Index()
        {
            var departments = await _departmentService.GetAllAsync();
            return View(departments);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadParentDepartmentsAsync();
            return View(new DepartmentCreateViewModel());
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
                DepartmentCode = model.DepartmentCode.Trim(),
                DepartmentName = model.DepartmentName.Trim(),
                Description = model.Description,
                ParentDepartmentId = model.ParentDepartmentId,
                IsActive = model.IsActive
            };

            await _departmentService.AddAsync(department);

            TempData["SuccessMessage"] = "Thêm khoa/ngành thành công.";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadParentDepartmentsAsync()
        {
            var parents = await _departmentService.GetParentDepartmentsAsync();

            ViewBag.ParentDepartments = parents.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = $"{d.DepartmentCode} - {d.DepartmentName}"
            }).ToList();
        }
    }
}