using Microsoft.AspNetCore.Mvc;
using QLKH.Application.Interfaces.Services;
using QLKH.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
namespace QLKH.Web.Areas.HocVu.Controllers
{
    [Area("HocVu")]
    [Authorize(Roles = "Admin,HocVu")]
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        public async Task<IActionResult> Index()
        {
            var students = await _studentService.GetAllAsync();
            return View(students);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            if (!ModelState.IsValid)
            {
                return View(student);
            }

            var result = await _studentService.CreateAsync(student);
            if (!result)
            {
                ModelState.AddModelError("", "Mã học viên đã tồn tại.");
                return View(student);
            }

            return RedirectToAction(nameof(Index), "Student", new { area = "HocVu" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Student student)
        {
            if (!ModelState.IsValid)
            {
                return View(student);
            }

            var result = await _studentService.UpdateAsync(student);
            if (!result)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index), "Student", new { area = "HocVu" });
        }
        [Authorize(Roles = "Admin,HocVu")]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }
        [Authorize(Roles = "Admin,HocVu")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _studentService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index), "Student", new { area = "HocVu" });
        }

        public async Task<IActionResult> Details(int id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }
    }
}
