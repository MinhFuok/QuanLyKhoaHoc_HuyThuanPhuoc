using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using QLKH.Application.Interfaces.Services;
using QLKH.Web.Areas.HocVu.Models;
using QLKH.Domain.Enums;
using ClosedXML.Excel;
using System.IO;

namespace QLKH.Web.Areas.GiaoVien.Controllers
{
    [Area("GiaoVien")]
    [Authorize(Roles = "GiaoVien")]
    public class MyClassesController : Controller
    {
        private readonly ITeacherService _teacherService;
        private readonly IClassRoomService _classRoomService;

        public MyClassesController(
            ITeacherService teacherService,
            IClassRoomService classRoomService)
        {
            _teacherService = teacherService;
            _classRoomService = classRoomService;
        }

        public async Task<IActionResult> Index()
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(applicationUserId))
            {
                return Challenge();
            }

            var classes = await _teacherService.GetMyTeachingClassesAsync(applicationUserId);
            return View(classes);
        }
        public async Task<IActionResult> Students(int id)
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(applicationUserId))
            {
                return Challenge();
            }

            var myClasses = await _teacherService.GetMyTeachingClassesAsync(applicationUserId);
            var isMyClass = myClasses.Any(x => x.Id == id);

            if (!isMyClass)
            {
                return Forbid();
            }

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
        public async Task<IActionResult> ExportStudentsToExcel(int id)
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(applicationUserId))
            {
                return Challenge();
            }

            var myClasses = await _teacherService.GetMyTeachingClassesAsync(applicationUserId);
            var isMyClass = myClasses.Any(x => x.Id == id);

            if (!isMyClass)
            {
                return Forbid();
            }

            var classRoom = await _classRoomService.GetByIdWithDetailsAsync(id);
            if (classRoom == null)
            {
                return NotFound();
            }

            var students = classRoom.Enrollments
                .Where(x => x.Student != null)
                .OrderBy(x => x.Student!.FullName)
                .Select(x => new
                {
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
                .ToList();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("DanhSachHocVien");

            worksheet.Cell(1, 1).Value = "DANH SÁCH HỌC VIÊN";
            worksheet.Cell(2, 1).Value = "Mã lớp:";
            worksheet.Cell(2, 2).Value = classRoom.ClassCode;
            worksheet.Cell(3, 1).Value = "Tên lớp:";
            worksheet.Cell(3, 2).Value = classRoom.ClassName;
            worksheet.Cell(4, 1).Value = "Khóa học:";
            worksheet.Cell(4, 2).Value = classRoom.Course?.CourseName ?? "";
            worksheet.Cell(5, 1).Value = "Giảng viên:";
            worksheet.Cell(5, 2).Value = classRoom.Teacher?.FullName ?? "";

            worksheet.Cell(7, 1).Value = "STT";
            worksheet.Cell(7, 2).Value = "Mã học viên";
            worksheet.Cell(7, 3).Value = "Họ tên";
            worksheet.Cell(7, 4).Value = "Email";
            worksheet.Cell(7, 5).Value = "Số điện thoại";
            worksheet.Cell(7, 6).Value = "Ngày ghi danh";
            worksheet.Cell(7, 7).Value = "Trạng thái";

            var headerRange = worksheet.Range(7, 1, 7, 7);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            int row = 8;
            int stt = 1;

            foreach (var student in students)
            {
                worksheet.Cell(row, 1).Value = stt;
                worksheet.Cell(row, 2).Value = student.StudentCode;
                worksheet.Cell(row, 3).Value = student.FullName;
                worksheet.Cell(row, 4).Value = student.Email;
                worksheet.Cell(row, 5).Value = student.PhoneNumber;
                worksheet.Cell(row, 6).Value = student.EnrolledAt.ToString("dd/MM/yyyy HH:mm");
                worksheet.Cell(row, 7).Value = student.StatusText;

                row++;
                stt++;
            }

            worksheet.Columns().AdjustToContents();

            var titleRange = worksheet.Range(1, 1, 1, 7);
            titleRange.Merge();
            titleRange.Style.Font.Bold = true;
            titleRange.Style.Font.FontSize = 16;
            titleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            var fileName = $"DanhSachHocVien_{classRoom.ClassCode}.xlsx";
            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
    }
}