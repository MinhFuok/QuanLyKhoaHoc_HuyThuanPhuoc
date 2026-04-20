using Microsoft.AspNetCore.Mvc;
using QLKH.Application.Interfaces.Services;
using QLKH.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
namespace QLKH.Web.Areas.HocVu.Controllers
{
    [Area("HocVu")]
    [Authorize(Roles = "Admin,HocVu")]
    public class TeacherController : Controller
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        public async Task<IActionResult> Index()
        {
            var teachers = await _teacherService.GetAllAsync();
            return View(teachers);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Teacher teacher)
        {
            if (!ModelState.IsValid)
            {
                return View(teacher);
            }

            var result = await _teacherService.CreateAsync(teacher);
            if (!result)
            {
                ModelState.AddModelError("", "Mã giảng viên đã tồn tại.");
                return View(teacher);
            }

            return RedirectToAction(nameof(Index), "Teacher", new { area = "HocVu" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var teacher = await _teacherService.GetByIdAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Teacher teacher)
        {
            if (!ModelState.IsValid)
            {
                return View(teacher);
            }

            var result = await _teacherService.UpdateAsync(teacher);
            if (!result)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index), "Teacher", new { area = "HocVu" });
        }
        [Authorize(Roles = "Admin,HocVu")]
        public async Task<IActionResult> Delete(int id)
        {
            var teacher = await _teacherService.GetByIdAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }
        [Authorize(Roles = "Admin,HocVu")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _teacherService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index), "Teacher", new { area = "HocVu" });
        }

        public async Task<IActionResult> Details(int id)
        {
            var teacher = await _teacherService.GetByIdAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }
    }
}
