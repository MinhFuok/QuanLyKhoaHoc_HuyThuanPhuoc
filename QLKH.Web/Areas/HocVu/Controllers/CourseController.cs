using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKH.Application.Interfaces.Services;
using QLKH.Domain.Entities;
using QLKH.Domain.Enums;

namespace QLKH.Web.Areas.HocVu.Controllers
{
    [Area("HocVu")]
    [Authorize(Roles = "Admin,HocVu")]
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        public async Task<IActionResult> Index()
        {
            var courses = await _courseService.GetAllAsync();
            return View(courses);
        }

        public IActionResult Create()
        {
            ViewBag.StatusList = Enum.GetValues(typeof(CourseStatus));
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.StatusList = Enum.GetValues(typeof(CourseStatus));
                return View(course);
            }

            var result = await _courseService.CreateAsync(course);
            if (!result)
            {
                ModelState.AddModelError("", "Mã khóa học đã tồn tại.");
                ViewBag.StatusList = Enum.GetValues(typeof(CourseStatus));
                return View(course);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            ViewBag.StatusList = Enum.GetValues(typeof(CourseStatus));
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Course course)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.StatusList = Enum.GetValues(typeof(CourseStatus));
                return View(course);
            }

            var result = await _courseService.UpdateAsync(course);
            if (!result)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var model = await _courseService.GetDeleteImpactAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int courseId, bool confirmCascadeDelete)
        {
            if (!confirmCascadeDelete)
            {
                TempData["ErrorMessage"] = "Bạn cần xác nhận đã hiểu các dữ liệu liên quan sẽ bị xóa.";
                return RedirectToAction(nameof(Delete), new { id = courseId });
            }

            var result = await _courseService.DeleteCascadeAsync(courseId);
            if (!result)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Xóa khóa học và toàn bộ dữ liệu liên quan thành công.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }
    }
}