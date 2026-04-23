using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLKH.Application.Interfaces.Services;
using QLKH.Application.ViewModels;
using QLKH.Domain.Entities;
using QLKH.Infrastructure.Identity;

namespace QLKH.Web.Areas.HocVu.Controllers
{
    [Area("HocVu")]
    [Authorize(Roles = "Admin,HocVu")]
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentController(
            IStudentService studentService,
            UserManager<ApplicationUser> userManager)
        {
            _studentService = studentService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var students = await _studentService.GetAllAsync();
            return View(students);
        }

        public async Task<IActionResult> Create()
        {
            await LoadAvailableStudentAccountsAsync();
            return View(new StudentFormViewModel
            {
                DateOfBirth = new DateTime(2005, 1, 1)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentFormViewModel model)
        {
            await ValidateLinkedAccountAsync(model.ApplicationUserId);

            if (await _studentService.ExistsByApplicationUserIdAsync(model.ApplicationUserId))
            {
                ModelState.AddModelError(nameof(model.ApplicationUserId), "Tài khoản này đã được liên kết với học viên khác.");
            }

            var linkedUser = await GetLinkedUserAsync(model.ApplicationUserId);
            if (linkedUser != null)
            {
                model.FullName = linkedUser.FullName ?? string.Empty;
                model.Email = linkedUser.Email ?? string.Empty;
            }
            model.PhoneNumber = (model.PhoneNumber ?? string.Empty).Trim();
            if (!ModelState.IsValid)
            {
                await LoadAvailableStudentAccountsAsync(model.ApplicationUserId);
                return View(model);
            }

            var student = new Student
            {
                StudentCode = model.StudentCode,
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                DateOfBirth = model.DateOfBirth,
                Address = model.Address,
                ApplicationUserId = model.ApplicationUserId
            };

            var result = await _studentService.CreateAsync(student);
            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Mã học viên đã tồn tại hoặc tài khoản đã được liên kết.");
                await LoadAvailableStudentAccountsAsync(model.ApplicationUserId);
                return View(model);
            }

            TempData["SuccessMessage"] = "Tạo học viên thành công.";
            return RedirectToAction(nameof(Index), "Student", new { area = "HocVu" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            var linkedUser = !string.IsNullOrWhiteSpace(student.ApplicationUserId)
                ? await _userManager.FindByIdAsync(student.ApplicationUserId)
                : null;

            await LoadAvailableStudentAccountsAsync(student.ApplicationUserId);

            var model = new StudentFormViewModel
            {
                Id = student.Id,
                StudentCode = student.StudentCode,
                FullName = student.FullName,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber,
                DateOfBirth = student.DateOfBirth,
                Address = student.Address,
                ApplicationUserId = student.ApplicationUserId ?? string.Empty,
                LinkedAccountEmail = linkedUser?.Email,
                LinkedAccountFullName = linkedUser?.FullName,
                LinkedAccountLocked = linkedUser != null && linkedUser.LockoutEnd.HasValue && linkedUser.LockoutEnd > DateTimeOffset.UtcNow
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentFormViewModel model)
        {
            await ValidateLinkedAccountAsync(model.ApplicationUserId);

            if (await _studentService.ExistsByApplicationUserIdAsync(model.ApplicationUserId, model.Id))
            {
                ModelState.AddModelError(nameof(model.ApplicationUserId), "Tài khoản này đã được liên kết với học viên khác.");
            }

            var linkedUser = await GetLinkedUserAsync(model.ApplicationUserId);
            if (linkedUser != null)
            {
                model.FullName = linkedUser.FullName ?? string.Empty;
                model.Email = linkedUser.Email ?? string.Empty;
            }
            model.PhoneNumber = (model.PhoneNumber ?? string.Empty).Trim();
            if (!ModelState.IsValid)
            {
                await LoadAvailableStudentAccountsAsync(model.ApplicationUserId);
                return View(model);
            }

            var student = new Student
            {
                Id = model.Id,
                StudentCode = model.StudentCode,
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                DateOfBirth = model.DateOfBirth,
                Address = model.Address,
                ApplicationUserId = model.ApplicationUserId
            };

            var result = await _studentService.UpdateAsync(student);
            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Không cập nhật được học viên hoặc tài khoản đã được liên kết với hồ sơ khác.");
                await LoadAvailableStudentAccountsAsync(model.ApplicationUserId);
                return View(model);
            }

            TempData["SuccessMessage"] = "Cập nhật học viên thành công.";
            return RedirectToAction(nameof(Index), "Student", new { area = "HocVu" });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            var linkedUser = !string.IsNullOrWhiteSpace(student.ApplicationUserId)
                ? await _userManager.FindByIdAsync(student.ApplicationUserId)
                : null;

            var model = new StudentFormViewModel
            {
                Id = student.Id,
                StudentCode = student.StudentCode,
                FullName = student.FullName,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber,
                DateOfBirth = student.DateOfBirth,
                Address = student.Address,
                ApplicationUserId = student.ApplicationUserId ?? string.Empty,
                LinkedAccountEmail = linkedUser?.Email,
                LinkedAccountFullName = linkedUser?.FullName,
                LinkedAccountLocked = linkedUser != null && linkedUser.LockoutEnd.HasValue && linkedUser.LockoutEnd > DateTimeOffset.UtcNow
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            ApplicationUser? linkedUser = null;
            if (!string.IsNullOrWhiteSpace(student.ApplicationUserId))
            {
                linkedUser = await _userManager.FindByIdAsync(student.ApplicationUserId);
            }

            var result = await _studentService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            if (linkedUser != null)
            {
                linkedUser.LockoutEnabled = true;
                await _userManager.SetLockoutEndDateAsync(linkedUser, DateTimeOffset.UtcNow.AddYears(100));
            }

            TempData["SuccessMessage"] = "Xóa học viên thành công. Tài khoản liên kết đã được vô hiệu hóa.";
            return RedirectToAction(nameof(Index), "Student", new { area = "HocVu" });
        }

        public async Task<IActionResult> Details(int id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            var linkedUser = !string.IsNullOrWhiteSpace(student.ApplicationUserId)
                ? await _userManager.FindByIdAsync(student.ApplicationUserId)
                : null;

            var model = new StudentFormViewModel
            {
                Id = student.Id,
                StudentCode = student.StudentCode,
                FullName = student.FullName,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber,
                DateOfBirth = student.DateOfBirth,
                Address = student.Address,
                ApplicationUserId = student.ApplicationUserId ?? string.Empty,
                LinkedAccountEmail = linkedUser?.Email,
                LinkedAccountFullName = linkedUser?.FullName,
                LinkedAccountLocked = linkedUser != null && linkedUser.LockoutEnd.HasValue && linkedUser.LockoutEnd > DateTimeOffset.UtcNow
            };

            return View(model);
        }

        private async Task LoadAvailableStudentAccountsAsync(string? selectedUserId = null)
        {
            var allHocVienUsers = await _userManager.GetUsersInRoleAsync("HocVien");
            var allStudents = (await _studentService.GetAllAsync()).ToList();

            var usedUserIds = allStudents
                .Where(x => !string.IsNullOrWhiteSpace(x.ApplicationUserId))
                .Select(x => x.ApplicationUserId!)
                .ToHashSet();

            var now = DateTimeOffset.UtcNow;

            var availableUsers = allHocVienUsers
                .Where(x =>
                    // chưa bị link hoặc chính là account đang được chọn ở màn hình Edit
                    (!usedUserIds.Contains(x.Id) || x.Id == selectedUserId)
                    // và không bị khóa
                    && (!x.LockoutEnd.HasValue || x.LockoutEnd <= now || x.Id == selectedUserId))
                .OrderBy(x => x.Email)
                .ToList();

            ViewBag.ApplicationUserId = availableUsers
                .Select(x => new SelectListItem
                {
                    Value = x.Id,
                    Text = $"{x.Email} - {x.FullName}"
                })
                .ToList();

            ViewBag.StudentAccountMeta = availableUsers
                .Select(x => new
                {
                    id = x.Id,
                    fullName = x.FullName ?? "",
                    email = x.Email ?? ""
                })
                .ToList();
        }

        private async Task ValidateLinkedAccountAsync(string applicationUserId)
        {
            if (string.IsNullOrWhiteSpace(applicationUserId))
            {
                ModelState.AddModelError(nameof(StudentFormViewModel.ApplicationUserId), "Vui lòng chọn tài khoản liên kết.");
                return;
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == applicationUserId);
            if (user == null)
            {
                ModelState.AddModelError(nameof(StudentFormViewModel.ApplicationUserId), "Tài khoản liên kết không tồn tại.");
                return;
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("HocVien"))
            {
                ModelState.AddModelError(nameof(StudentFormViewModel.ApplicationUserId), "Chỉ được liên kết với tài khoản có vai trò Học viên.");
            }
        }
        // Đổ in4 tên và gmail 
        private async Task<ApplicationUser?> GetLinkedUserAsync(string applicationUserId)
        {
            if (string.IsNullOrWhiteSpace(applicationUserId))
            {
                return null;
            }

            return await _userManager.Users.FirstOrDefaultAsync(x => x.Id == applicationUserId);
        }
    }
}