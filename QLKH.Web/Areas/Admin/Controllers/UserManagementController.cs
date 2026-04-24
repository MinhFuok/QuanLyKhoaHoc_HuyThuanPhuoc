using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLKH.Application.Interfaces.Services;
using QLKH.Infrastructure.Identity;
using QLKH.Web.Areas.Admin.Models;
using QLKH.Web.Services;

namespace QLKH.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStudentService _studentService;
        private readonly ITeacherService _teacherService;
        private readonly IEmailSenderService _emailSenderService;
        private readonly IAdminAuditLogService _adminAuditLogService;

        public UserManagementController(
            UserManager<ApplicationUser> userManager,
            IStudentService studentService,
            ITeacherService teacherService,
            IEmailSenderService emailSenderService,
            IAdminAuditLogService adminAuditLogService)
        {
            _userManager = userManager;
            _studentService = studentService;
            _teacherService = teacherService;
            _emailSenderService = emailSenderService;
            _adminAuditLogService = adminAuditLogService;
        }

        public async Task<IActionResult> Index(string? keyword, string? role, string? status, string? linkStatus)
        {
            var users = await _userManager.Users
                .OrderBy(u => u.Email)
                .ToListAsync();

            var model = new List<UserListItemViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userRole = roles.FirstOrDefault() ?? string.Empty;

                var student = await _studentService.GetByApplicationUserIdAsync(user.Id);
                var teacher = await _teacherService.GetByApplicationUserIdAsync(user.Id);

                var linkedLabel = "Chưa liên kết";
                if (student != null)
                {
                    linkedLabel = $"Học viên - {student.StudentCode}";
                }
                else if (teacher != null)
                {
                    linkedLabel = $"Giáo viên - {teacher.TeacherCode}";
                }

                model.Add(new UserListItemViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    Role = userRole,
                    IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow,
                    IsLinkedStudent = student != null,
                    IsLinkedTeacher = teacher != null,
                    LinkedLabel = linkedLabel
                });
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim().ToLowerInvariant();
                model = model
                    .Where(x =>
                        x.FullName.ToLowerInvariant().Contains(keyword) ||
                        x.UserName.ToLowerInvariant().Contains(keyword) ||
                        x.Email.ToLowerInvariant().Contains(keyword))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                model = model
                    .Where(x => x.Role.Equals(role, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                model = status switch
                {
                    "active" => model.Where(x => !x.IsLocked).ToList(),
                    "locked" => model.Where(x => x.IsLocked).ToList(),
                    _ => model
                };
            }

            if (!string.IsNullOrWhiteSpace(linkStatus))
            {
                model = linkStatus switch
                {
                    "linked" => model.Where(x => x.IsLinkedStudent || x.IsLinkedTeacher).ToList(),
                    "unlinked" => model.Where(x => !x.IsLinkedStudent && !x.IsLinkedTeacher).ToList(),
                    _ => model
                };
            }

            ViewBag.Keyword = keyword;
            ViewBag.Role = role;
            ViewBag.Status = status;
            ViewBag.LinkStatus = linkStatus;

            ViewBag.RoleList = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "-- Tất cả vai trò --" },
                new SelectListItem { Value = "Admin", Text = "Admin" },
                new SelectListItem { Value = "HocVu", Text = "Học vụ" },
                new SelectListItem { Value = "GiaoVien", Text = "Giáo viên" },
                new SelectListItem { Value = "HocVien", Text = "Học viên" }
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            LoadRoles();
            return View(new CreateUserViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            LoadRoles();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var normalizedEmail = model.Email.Trim();
            var existingUser = await _userManager.FindByEmailAsync(normalizedEmail);
            if (existingUser != null)
            {
                ModelState.AddModelError(nameof(model.Email), "Email này đã tồn tại.");
                return View(model);
            }

            var user = new ApplicationUser
            {
                FullName = model.FullName.Trim(),
                Email = normalizedEmail,
                UserName = normalizedEmail,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, model.Role);
            if (!addRoleResult.Succeeded)
            {
                foreach (var error in addRoleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                await _userManager.DeleteAsync(user);
                return View(model);
            }

            await _adminAuditLogService.WriteAsync(
                CurrentAdminId,
                CurrentAdminEmail,
                "CREATE_USER",
                "User",
                user.Id,
                $"{user.FullName} - {user.Email}",
                $"Vai trò: {model.Role}");

            try
            {
                await SendAccountCreatedEmailAsync(user.Email!, user.FullName ?? string.Empty, model.Password);
                TempData["SuccessMessage"] = "Tạo tài khoản thành công và đã gửi email thông tin đăng nhập.";
            }
            catch
            {
                TempData["ErrorMessage"] = "Tạo tài khoản thành công nhưng gửi email thất bại. Vui lòng kiểm tra cấu hình Email hệ thống.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Role = roles.FirstOrDefault() ?? string.Empty
            };

            LoadRoles();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            LoadRoles();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            var normalizedEmail = model.Email.Trim();
            var duplicatedEmailUser = await _userManager.FindByEmailAsync(normalizedEmail);
            if (duplicatedEmailUser != null && duplicatedEmailUser.Id != model.Id)
            {
                ModelState.AddModelError(nameof(model.Email), "Email này đã tồn tại.");
                return View(model);
            }

            user.FullName = model.FullName.Trim();
            user.Email = normalizedEmail;
            user.UserName = normalizedEmail;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            if (currentRoles.Any())
            {
                var removeRoleResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeRoleResult.Succeeded)
                {
                    foreach (var error in removeRoleResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View(model);
                }
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, model.Role);
            if (!addRoleResult.Succeeded)
            {
                foreach (var error in addRoleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            await _adminAuditLogService.WriteAsync(
                CurrentAdminId,
                CurrentAdminEmail,
                "EDIT_USER",
                "User",
                user.Id,
                $"{user.FullName} - {user.Email}",
                $"Vai trò mới: {model.Role}");

            TempData["SuccessMessage"] = "Cập nhật tài khoản thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var student = await _studentService.GetByApplicationUserIdAsync(user.Id);
            var teacher = await _teacherService.GetByApplicationUserIdAsync(user.Id);

            var model = new DeleteUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Role = roles.FirstOrDefault() ?? string.Empty,
                IsLinkedStudent = student != null,
                IsLinkedTeacher = teacher != null,
                LinkedStudentCode = student?.StudentCode,
                LinkedTeacherCode = teacher?.TeacherCode
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (string.Equals(user.Email, User.Identity?.Name, StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Bạn không thể tự xóa tài khoản của chính mình.";
                return RedirectToAction(nameof(Index));
            }

            var student = await _studentService.GetByApplicationUserIdAsync(user.Id);
            var teacher = await _teacherService.GetByApplicationUserIdAsync(user.Id);

            if (student != null || teacher != null)
            {
                TempData["ErrorMessage"] = "Không thể xóa tài khoản vì đang liên kết với hồ sơ nghiệp vụ.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                await _adminAuditLogService.WriteAsync(
                    CurrentAdminId,
                    CurrentAdminEmail,
                    "DELETE_USER",
                    "User",
                    user.Id,
                    $"{user.FullName} - {user.Email}");

                TempData["SuccessMessage"] = "Xóa tài khoản thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Xóa tài khoản thất bại.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (string.Equals(user.Email, User.Identity?.Name, StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Bạn không thể tự khóa tài khoản của chính mình.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));

            if (result.Succeeded)
            {
                await _adminAuditLogService.WriteAsync(
                    CurrentAdminId,
                    CurrentAdminEmail,
                    "LOCK_USER",
                    "User",
                    user.Id,
                    $"{user.FullName} - {user.Email}");

                TempData["SuccessMessage"] = "Khóa tài khoản thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Khóa tài khoản thất bại.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlockUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.SetLockoutEndDateAsync(user, null);

            if (result.Succeeded)
            {
                await _adminAuditLogService.WriteAsync(
                    CurrentAdminId,
                    CurrentAdminEmail,
                    "UNLOCK_USER",
                    "User",
                    user.Id,
                    $"{user.FullName} - {user.Email}");

                TempData["SuccessMessage"] = "Mở khóa tài khoản thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Mở khóa tài khoản thất bại.";
            }

            return RedirectToAction(nameof(Index));
        }

        private void LoadRoles()
        {
            ViewBag.RoleList = new List<SelectListItem>
            {
                new SelectListItem { Value = "Admin", Text = "Admin" },
                new SelectListItem { Value = "HocVu", Text = "Học vụ" },
                new SelectListItem { Value = "GiaoVien", Text = "Giáo viên" },
                new SelectListItem { Value = "HocVien", Text = "Học viên" }
            };
        }

        private string CurrentAdminEmail => User.Identity?.Name ?? "Unknown Admin";
        private string? CurrentAdminId => _userManager.GetUserId(User);

        private async Task SendAccountCreatedEmailAsync(string email, string fullName, string password)
        {
            var subject = "Thông tin tài khoản đăng nhập hệ thống QLKH";

            var body = $@"
<div style='font-family:Arial,Helvetica,sans-serif; font-size:14px; line-height:1.6; color:#222;'>
    <p>Xin chào <strong>{fullName}</strong>,</p>

    <p>Tài khoản của bạn trên hệ thống <strong>QLKH</strong> đã được tạo thành công.</p>

    <p><strong>Thông tin đăng nhập:</strong></p>
    <ul>
        <li>Username: <strong>{email}</strong></li>
        <li>Password: <strong>{password}</strong></li>
    </ul>

    <p><strong>Lưu ý:</strong></p>
    <ul>
        <li>Bạn nên đổi mật khẩu sau khi đăng nhập lần đầu để bảo mật tài khoản.</li>
        <li>Vui lòng không chia sẻ thông tin đăng nhập cho người khác.</li>
    </ul>

    <p>Trân trọng,<br/>Hệ thống QLKH</p>
</div>";

            await _emailSenderService.SendEmailAsync(email, subject, body);
        }
    }
}