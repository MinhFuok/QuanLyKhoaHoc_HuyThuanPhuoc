using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QLKH.Application.Interfaces.Services;
using QLKH.Infrastructure.Identity;
using QLKH.Web.Areas.HocVien.Models;

namespace QLKH.Web.Areas.HocVien.Controllers
{
    [Area("HocVien")]
    [Authorize(Roles = "Admin,HocVien")]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStudentService _studentService;
        private readonly IStudentCertificateService _studentCertificateService;

        public HomeController(
            UserManager<ApplicationUser> userManager,
            IStudentService studentService,
            IStudentCertificateService studentCertificateService)
        {
            _userManager = userManager;
            _studentService = studentService;
            _studentCertificateService = studentCertificateService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HocVienDashboardViewModel();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return View(model);
            }

            var students = await _studentService.GetAllAsync();
            var student = students.FirstOrDefault(x => x.Email == user.Email);

            if (student != null)
            {
                model.FullName = student.FullName;

                var certificates = await _studentCertificateService.GetByStudentIdAsync(student.Id);
                model.TotalCertificates = certificates.Count();
                model.TotalActiveCertificates = certificates.Count(x => x.IsApproved);
            }
            else
            {
                model.FullName = user.FullName ?? user.Email ?? "Học viên";
            }

            return View(model);
        }
    }
}