using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKH.Application.Interfaces.Services;
using System.Security.Claims;

namespace QLKH.Web.Areas.HocVien.Controllers
{
    [Area("HocVien")]
    [Authorize(Roles = "Admin,HocVien")]
    public class MyClassesController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly ITeacherReviewService _teacherReviewService;

        public MyClassesController(
            IStudentService studentService,
            ITeacherReviewService teacherReviewService)
        {
            _studentService = studentService;
            _teacherReviewService = teacherReviewService;
        }

        public async Task<IActionResult> Index()
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(applicationUserId))
            {
                return Challenge();
            }

            var classes = (await _studentService.GetMyClassesAsync(applicationUserId)).ToList();

            var reviewedClassIds = new List<int>();

            foreach (var classRoom in classes)
            {
                var review = await _teacherReviewService.GetMyReviewForClassAsync(
                    applicationUserId,
                    classRoom.Id);

                if (review != null)
                {
                    reviewedClassIds.Add(classRoom.Id);
                }
            }

            ViewBag.ReviewedClassIds = reviewedClassIds;

            return View(classes);
        }
    }
}