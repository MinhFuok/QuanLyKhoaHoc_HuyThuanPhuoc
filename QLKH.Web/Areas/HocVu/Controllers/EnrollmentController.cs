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
            return View(new Enrollment
            {
                Status = EnrollmentStatus.Pending
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Enrollment enrollment)
        {
            enrollment.Status = EnrollmentStatus.Pending;

            if (!ModelState.IsValid)
            {
                await LoadDropdownDataAsync(enrollment.StudentId, enrollment.ClassRoomId);
                return View(enrollment);
            }

            var result = await _enrollmentService.CreateAsync(enrollment);
            if (!result)
            {
                ModelState.AddModelError("", "Không thể ghi danh. Có thể học viên đã tồn tại trong lớp.");
                await LoadDropdownDataAsync(enrollment.StudentId, enrollment.ClassRoomId);
                return View(enrollment);
            }

            TempData["SuccessMessage"] = "Tạo ghi danh thành công. Ghi danh đang ở trạng thái chờ xác nhận.";
            return RedirectToAction(nameof(Index), new { tab = "pending" });
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

        public async Task<IActionResult> Details(int id)
        {
            var enrollment = await _enrollmentService.GetByIdWithDetailsAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var enrollment = await _enrollmentService.GetByIdWithDetailsAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int id)
        {
            var result = await _enrollmentService.ChangeStatusAsync(id, EnrollmentStatus.Confirmed);
            if (!result)
            {
                TempData["ErrorMessage"] = "Không thể xác nhận ghi danh. Có thể lớp đã đủ sĩ số.";
            }
            else
            {
                TempData["SuccessMessage"] = "Xác nhận ghi danh thành công.";
            }

            return RedirectToAction(nameof(Index), new { tab = "confirmed" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _enrollmentService.ChangeStatusAsync(id, EnrollmentStatus.Cancelled);
            if (!result)
            {
                TempData["ErrorMessage"] = "Không thể hủy ghi danh.";
            }
            else
            {
                TempData["SuccessMessage"] = "Hủy ghi danh thành công.";
            }

            return RedirectToAction(nameof(Index), new { tab = "cancelled" });
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