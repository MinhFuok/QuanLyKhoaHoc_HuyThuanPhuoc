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
    public class TeacherController : Controller
    {
        private readonly ITeacherService _teacherService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TeacherController(
            ITeacherService teacherService,
            UserManager<ApplicationUser> userManager)
        {
            _teacherService = teacherService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var teachers = await _teacherService.GetAllAsync();
            return View(teachers);
        }

        public async Task<IActionResult> Create()
        {
            await LoadAvailableTeacherAccountsAsync();
            return View(new TeacherFormViewModel
            {
                TeacherCode = await _teacherService.GenerateNextTeacherCodeAsync()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeacherFormViewModel model)
        {
            model.PhoneNumber = (model.PhoneNumber ?? string.Empty).Trim();
            model.TeacherCode = await _teacherService.GenerateNextTeacherCodeAsync();
            await ValidateLinkedAccountAsync(model.ApplicationUserId);

            if (await _teacherService.ExistsByApplicationUserIdAsync(model.ApplicationUserId))
            {
                ModelState.AddModelError(nameof(model.ApplicationUserId), "Tài khoản này đã được liên kết với giáo viên khác.");
            }

            var linkedUser = await GetLinkedUserAsync(model.ApplicationUserId);
            if (linkedUser != null)
            {
                model.FullName = linkedUser.FullName ?? string.Empty;
                model.Email = linkedUser.Email ?? string.Empty;
            }

            if (!ModelState.IsValid)
            {
                await LoadAvailableTeacherAccountsAsync(model.ApplicationUserId);
                return View(model);
            }

            var teacher = new Teacher
            {
                TeacherCode = model.TeacherCode,
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Specialization = model.Specialization,
                ApplicationUserId = model.ApplicationUserId
            };

            var result = await _teacherService.CreateAsync(teacher);
            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Mã giáo viên đã tồn tại hoặc tài khoản đã được liên kết.");
                await LoadAvailableTeacherAccountsAsync(model.ApplicationUserId);
                return View(model);
            }

            TempData["SuccessMessage"] = "Tạo giáo viên thành công.";
            return RedirectToAction(nameof(Index), "Teacher", new { area = "HocVu" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var teacher = await _teacherService.GetByIdAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }

            var linkedUser = !string.IsNullOrWhiteSpace(teacher.ApplicationUserId)
                ? await _userManager.FindByIdAsync(teacher.ApplicationUserId)
                : null;

            await LoadAvailableTeacherAccountsAsync(teacher.ApplicationUserId);

            var model = new TeacherFormViewModel
            {
                Id = teacher.Id,
                TeacherCode = teacher.TeacherCode,
                FullName = teacher.FullName,
                Email = teacher.Email,
                PhoneNumber = teacher.PhoneNumber,
                Specialization = teacher.Specialization,
                ApplicationUserId = teacher.ApplicationUserId ?? string.Empty,
                LinkedAccountEmail = linkedUser?.Email,
                LinkedAccountFullName = linkedUser?.FullName,
                LinkedAccountLocked = linkedUser != null && linkedUser.LockoutEnd.HasValue && linkedUser.LockoutEnd > DateTimeOffset.UtcNow
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TeacherFormViewModel model)
        {
            model.PhoneNumber = (model.PhoneNumber ?? string.Empty).Trim();

            await ValidateLinkedAccountAsync(model.ApplicationUserId);

            if (await _teacherService.ExistsByApplicationUserIdAsync(model.ApplicationUserId, model.Id))
            {
                ModelState.AddModelError(nameof(model.ApplicationUserId), "Tài khoản này đã được liên kết với giáo viên khác.");
            }

            var linkedUser = await GetLinkedUserAsync(model.ApplicationUserId);
            if (linkedUser != null)
            {
                model.FullName = linkedUser.FullName ?? string.Empty;
                model.Email = linkedUser.Email ?? string.Empty;
            }

            if (!ModelState.IsValid)
            {
                await LoadAvailableTeacherAccountsAsync(model.ApplicationUserId);
                return View(model);
            }

            var teacher = new Teacher
            {
                Id = model.Id,
                TeacherCode = model.TeacherCode,
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Specialization = model.Specialization,
                ApplicationUserId = model.ApplicationUserId
            };

            var result = await _teacherService.UpdateAsync(teacher);
            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Không cập nhật được giáo viên hoặc tài khoản đã được liên kết với hồ sơ khác.");
                await LoadAvailableTeacherAccountsAsync(model.ApplicationUserId);
                return View(model);
            }

            TempData["SuccessMessage"] = "Cập nhật giáo viên thành công.";
            return RedirectToAction(nameof(Index), "Teacher", new { area = "HocVu" });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var teacher = await _teacherService.GetByIdAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }

            var linkedUser = !string.IsNullOrWhiteSpace(teacher.ApplicationUserId)
                ? await _userManager.FindByIdAsync(teacher.ApplicationUserId)
                : null;

            var model = new TeacherFormViewModel
            {
                Id = teacher.Id,
                TeacherCode = teacher.TeacherCode,
                FullName = teacher.FullName,
                Email = teacher.Email,
                PhoneNumber = teacher.PhoneNumber,
                Specialization = teacher.Specialization,
                ApplicationUserId = teacher.ApplicationUserId ?? string.Empty,
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
            var teacher = await _teacherService.GetByIdAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }

            ApplicationUser? linkedUser = null;
            if (!string.IsNullOrWhiteSpace(teacher.ApplicationUserId))
            {
                linkedUser = await _userManager.FindByIdAsync(teacher.ApplicationUserId);
            }

            var result = await _teacherService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            if (linkedUser != null)
            {
                linkedUser.LockoutEnabled = true;
                await _userManager.SetLockoutEndDateAsync(linkedUser, DateTimeOffset.UtcNow.AddYears(100));
            }

            TempData["SuccessMessage"] = "Xóa giáo viên thành công. Tài khoản liên kết đã được vô hiệu hóa.";
            return RedirectToAction(nameof(Index), "Teacher", new { area = "HocVu" });
        }

        public async Task<IActionResult> Details(int id)
        {
            var teacher = await _teacherService.GetByIdAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }

            var linkedUser = !string.IsNullOrWhiteSpace(teacher.ApplicationUserId)
                ? await _userManager.FindByIdAsync(teacher.ApplicationUserId)
                : null;

            var model = new TeacherFormViewModel
            {
                Id = teacher.Id,
                TeacherCode = teacher.TeacherCode,
                FullName = teacher.FullName,
                Email = teacher.Email,
                PhoneNumber = teacher.PhoneNumber,
                Specialization = teacher.Specialization,
                ApplicationUserId = teacher.ApplicationUserId ?? string.Empty,
                LinkedAccountEmail = linkedUser?.Email,
                LinkedAccountFullName = linkedUser?.FullName,
                LinkedAccountLocked = linkedUser != null && linkedUser.LockoutEnd.HasValue && linkedUser.LockoutEnd > DateTimeOffset.UtcNow
            };

            return View(model);
        }

        private async Task<ApplicationUser?> GetLinkedUserAsync(string applicationUserId)
        {
            if (string.IsNullOrWhiteSpace(applicationUserId))
            {
                return null;
            }

            return await _userManager.Users.FirstOrDefaultAsync(x => x.Id == applicationUserId);
        }

        private async Task LoadAvailableTeacherAccountsAsync(string? selectedUserId = null)
        {
            var allGiaoVienUsers = await _userManager.GetUsersInRoleAsync("GiaoVien");
            var allTeachers = (await _teacherService.GetAllAsync()).ToList();

            var usedUserIds = allTeachers
                .Where(x => !string.IsNullOrWhiteSpace(x.ApplicationUserId))
                .Select(x => x.ApplicationUserId!)
                .ToHashSet();

            var now = DateTimeOffset.UtcNow;

            var availableUsers = allGiaoVienUsers
                .Where(x =>
                    (!usedUserIds.Contains(x.Id) || x.Id == selectedUserId) &&
                    (!x.LockoutEnd.HasValue || x.LockoutEnd <= now || x.Id == selectedUserId))
                .OrderBy(x => x.Email)
                .ToList();

            ViewBag.ApplicationUserId = availableUsers
                .Select(x => new SelectListItem
                {
                    Value = x.Id,
                    Text = $"{x.Email} - {x.FullName}"
                })
                .ToList();

            ViewBag.TeacherAccountMeta = availableUsers
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
                ModelState.AddModelError(nameof(TeacherFormViewModel.ApplicationUserId), "Vui lòng chọn tài khoản liên kết.");
                return;
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == applicationUserId);
            if (user == null)
            {
                ModelState.AddModelError(nameof(TeacherFormViewModel.ApplicationUserId), "Tài khoản liên kết không tồn tại.");
                return;
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("GiaoVien"))
            {
                ModelState.AddModelError(nameof(TeacherFormViewModel.ApplicationUserId), "Chỉ được liên kết với tài khoản có vai trò Giáo viên.");
            }
        }
    }
}