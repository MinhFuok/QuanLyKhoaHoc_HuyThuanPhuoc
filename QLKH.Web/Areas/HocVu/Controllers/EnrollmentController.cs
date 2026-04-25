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

        public async Task<IActionResult> Index(int? classRoomId, string? tab)
        {
            var enrollments = await _enrollmentService.GetAllAsync();

            if (classRoomId.HasValue && classRoomId.Value > 0)
            {
                enrollments = enrollments
                    .Where(x => x.ClassRoomId == classRoomId.Value)
                    .ToList();
            }

            await LoadIndexFilterDataAsync(classRoomId);

            ViewBag.SelectedClassRoomId = classRoomId;
            ViewBag.ActiveTab = tab;

            return View(enrollments);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? classRoomId)
        {
            await LoadCreateDataAsync(classRoomId);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int classRoomId, int[] studentIds)
        {
            if (classRoomId <= 0)
            {
                ModelState.AddModelError("", "Vui lòng chọn lớp học.");
                await LoadCreateDataAsync(classRoomId);
                return View();
            }

            if (studentIds == null || studentIds.Length == 0)
            {
                ModelState.AddModelError("", "Vui lòng chọn ít nhất một học viên để ghi danh.");
                await LoadCreateDataAsync(classRoomId);
                return View();
            }

            var successCount = 0;
            var failedCount = 0;

            foreach (var studentId in studentIds.Distinct())
            {
                var enrollment = new Enrollment
                {
                    StudentId = studentId,
                    ClassRoomId = classRoomId,
                    Status = EnrollmentStatus.Pending
                };

                var result = await _enrollmentService.CreateAsync(enrollment);

                if (result)
                {
                    successCount++;
                }
                else
                {
                    failedCount++;
                }
            }

            if (successCount == 0)
            {
                ModelState.AddModelError("", "Không thể ghi danh. Có thể các học viên đã tồn tại trong lớp hoặc lớp không hợp lệ.");
                await LoadCreateDataAsync(classRoomId);
                return View();
            }

            if (failedCount > 0)
            {
                TempData["SuccessMessage"] = $"Đã ghi danh {successCount} học viên. Có {failedCount} học viên không thể ghi danh do đã tồn tại hoặc dữ liệu không hợp lệ.";
            }
            else
            {
                TempData["SuccessMessage"] = $"Đã ghi danh thành công {successCount} học viên. Ghi danh đang ở trạng thái chờ xác nhận.";
            }

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

        private async Task LoadCreateDataAsync(int? selectedClassRoomId = null)
        {
            var students = await _studentService.GetAllAsync();
            var classRooms = await _classRoomService.GetAllAsync();

            ViewBag.Students = students;
            ViewBag.ClassRooms = classRooms;
            ViewBag.SelectedClassRoomId = selectedClassRoomId;
        }

        private async Task LoadDropdownDataAsync(int? selectedStudentId = null, int? selectedClassRoomId = null)
        {
            var students = await _studentService.GetAllAsync();
            var classRooms = await _classRoomService.GetAllAsync();

            ViewBag.StudentId = new SelectList(students, "Id", "FullName", selectedStudentId);
            ViewBag.ClassRoomId = new SelectList(classRooms, "Id", "ClassName", selectedClassRoomId);
        }
        private async Task LoadIndexFilterDataAsync(int? selectedClassRoomId = null)
        {
            var classRooms = await _classRoomService.GetAllAsync();

            ViewBag.ClassRoomFilter = new SelectList(
                classRooms,
                "Id",
                "ClassName",
                selectedClassRoomId
            );
        }
    }
}