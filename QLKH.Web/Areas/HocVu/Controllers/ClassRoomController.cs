using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QLKH.Application.Interfaces.Services;
using QLKH.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using QLKH.Web.Areas.HocVu.Models;
using QLKH.Domain.Enums;

namespace QLKH.Web.Areas.HocVu.Controllers
{
    [Area("HocVu")]
    [Authorize(Roles = "Admin,HocVu")]
    public class ClassRoomController : Controller
    {
        private readonly IClassRoomService _classRoomService;
        private readonly ICourseService _courseService;
        private readonly ITeacherService _teacherService;

        public ClassRoomController(
            IClassRoomService classRoomService,
            ICourseService courseService,
            ITeacherService teacherService)
        {
            _classRoomService = classRoomService;
            _courseService = courseService;
            _teacherService = teacherService;
        }
        private async Task<string> GenerateNextClassCodeAsync()
        {
            var classRooms = await _classRoomService.GetAllAsync();

            var lastCode = classRooms
                .Where(x => !string.IsNullOrWhiteSpace(x.ClassCode) && x.ClassCode.StartsWith("LH"))
                .OrderByDescending(x => x.ClassCode)
                .Select(x => x.ClassCode)
                .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(lastCode))
            {
                return "LH001";
            }

            var numberPart = lastCode.Substring(2);

            if (!int.TryParse(numberPart, out int number))
            {
                return "LH001";
            }

            return $"LH{(number + 1):D3}";
        }
        public async Task<IActionResult> Index()
        {
            var classRooms = await _classRoomService.GetAllAsync();
            return View(classRooms);
        }
        public async Task<IActionResult> Students(int id)
        {
            var classRoom = await _classRoomService.GetByIdWithDetailsAsync(id);
            if (classRoom == null)
            {
                return NotFound();
            }

            var model = new ClassStudentListViewModel
            {
                ClassRoomId = classRoom.Id,
                ClassCode = classRoom.ClassCode,
                ClassName = classRoom.ClassName,
                CourseName = classRoom.Course?.CourseName ?? string.Empty,
                TeacherName = classRoom.Teacher?.FullName ?? string.Empty,
                Students = classRoom.Enrollments
                    .Where(x => x.Student != null)
                    .OrderBy(x => x.Student!.FullName)
                    .Select(x => new ClassStudentListItemViewModel
                    {
                        EnrollmentId = x.Id,
                        StudentId = x.StudentId,
                        StudentCode = x.Student!.StudentCode,
                        FullName = x.Student.FullName,
                        Email = x.Student.Email,
                        PhoneNumber = x.Student.PhoneNumber,
                        EnrolledAt = x.EnrolledAt,
                        StatusText = x.Status switch
                        {
                            EnrollmentStatus.Pending => "Chờ duyệt",
                            EnrollmentStatus.Confirmed => "Đã duyệt",
                            EnrollmentStatus.Cancelled => "Đã hủy",
                            _ => x.Status.ToString()
                        }
                    })
                    .ToList()
            };

            return View(model);
        }
        public async Task<IActionResult> Create()
        {
            await LoadDropdownDataAsync();

            var model = new ClassRoom
            {
                ClassCode = await GenerateNextClassCodeAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClassRoom classRoom)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownDataAsync(classRoom.CourseId, classRoom.TeacherId);
                return View(classRoom);
            }
            classRoom.ClassCode = await GenerateNextClassCodeAsync();
            var result = await _classRoomService.CreateAsync(classRoom);
            if (!result)
            {
                ModelState.AddModelError("", "Không thể tạo lớp học. Kiểm tra lại mã lớp, khóa học hoặc giảng viên.");
                await LoadDropdownDataAsync(classRoom.CourseId, classRoom.TeacherId);
                return View(classRoom);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var classRoom = await _classRoomService.GetByIdAsync(id);
            if (classRoom == null)
            {
                return NotFound();
            }

            await LoadDropdownDataAsync(classRoom.CourseId, classRoom.TeacherId);
            return View(classRoom);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClassRoom classRoom)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownDataAsync(classRoom.CourseId, classRoom.TeacherId);
                return View(classRoom);
            }

            var result = await _classRoomService.UpdateAsync(classRoom);
            if (!result)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,HocVu")]
        public async Task<IActionResult> Delete(int id)
        {
            var classRoom = await _classRoomService.GetByIdWithDetailsAsync(id);
            if (classRoom == null)
            {
                return NotFound();
            }

            return View(classRoom);
        }
        [Authorize(Roles = "Admin,HocVu")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _classRoomService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var classRoom = await _classRoomService.GetByIdWithDetailsAsync(id);
            if (classRoom == null)
            {
                return NotFound();
            }

            return View(classRoom);
        }

        private async Task LoadDropdownDataAsync(int? selectedCourseId = null, int? selectedTeacherId = null)
        {
            var courses = await _courseService.GetAllAsync();
            var teachers = await _teacherService.GetAllAsync();

            ViewBag.CourseId = new SelectList(courses, "Id", "CourseName", selectedCourseId);
            ViewBag.TeacherId = new SelectList(teachers, "Id", "FullName", selectedTeacherId);
        }
    }
}
