using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QLKH.Application.Interfaces.Services;
using QLKH.Domain.Entities;
using QLKH.Domain.Enums;

namespace QLKH.Web.Areas.HocVu.Controllers
{
    [Area("HocVu")]
    [Authorize(Roles = "Admin,HocVu")]
    public class EnrollmentController : Controller
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IStudentService _studentService;
        private readonly IClassRoomService _classRoomService;

        public EnrollmentController(
            IEnrollmentService enrollmentService,
            IStudentService studentService,
            IClassRoomService classRoomService)
        {
            _enrollmentService = enrollmentService;
            _studentService = studentService;
            _classRoomService = classRoomService;
        }

        public async Task<IActionResult> Index()
        {
            var enrollments = await _enrollmentService.GetAllAsync();
            return View(enrollments);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdownDataAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Enrollment enrollment)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownDataAsync(enrollment.StudentId, enrollment.ClassRoomId);
                return View(enrollment);
            }

            enrollment.EnrolledAt = DateTime.Now;

            var result = await _enrollmentService.CreateAsync(enrollment);
            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Không thể ghi danh. Có thể học viên đã tồn tại trong lớp hoặc lớp đã đủ sĩ số.");
                await LoadDropdownDataAsync(enrollment.StudentId, enrollment.ClassRoomId);
                return View(enrollment);
            }

            TempData["SuccessMessage"] = "Ghi danh thành công.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var enrollment = await _enrollmentService.GetByIdAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }

            await LoadDropdownDataAsync(enrollment.StudentId, enrollment.ClassRoomId);
            ViewBag.StatusList = Enum.GetValues(typeof(EnrollmentStatus));
            return View(enrollment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Enrollment enrollment)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownDataAsync(enrollment.StudentId, enrollment.ClassRoomId);
                ViewBag.StatusList = Enum.GetValues(typeof(EnrollmentStatus));
                return View(enrollment);
            }

            var result = await _enrollmentService.UpdateAsync(enrollment);
            if (!result)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Cập nhật ghi danh thành công.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,HocVu")]
        public async Task<IActionResult> Delete(int id)
        {
            var enrollment = await _enrollmentService.GetByIdWithDetailsAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        [Authorize(Roles = "Admin,HocVu")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _enrollmentService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Xóa ghi danh thành công.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var enrollment = await _enrollmentService.GetByIdWithDetailsAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        private async Task LoadDropdownDataAsync(int? selectedStudentId = null, int? selectedClassRoomId = null)
        {
            var students = await _studentService.GetAllAsync();
            var classRooms = await _classRoomService.GetAllAsync();

            ViewBag.StudentId = new SelectList(students, "Id", "FullName", selectedStudentId);
            ViewBag.ClassRoomId = new SelectList(classRooms, "Id", "ClassName", selectedClassRoomId);
        }
    }
}