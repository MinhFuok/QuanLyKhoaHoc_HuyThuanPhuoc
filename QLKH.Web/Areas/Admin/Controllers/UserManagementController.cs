using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public UserManagementController(
            UserManager<ApplicationUser> userManager,
            IStudentService studentService,
            ITeacherService teacherService,
            IEmailSenderService emailSenderService)
        {
            _userManager = userManager;
            _studentService = studentService;
            _teacherService = teacherService;
            _emailSenderService = emailSenderService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                .OrderBy(u => u.Email)
                .ToListAsync();

            var model = new List<UserListItemViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                model.Add(new UserListItemViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName ?? "",
                    Email = user.Email ?? "",
                    UserName = user.UserName ?? "",
                    Role = roles.FirstOrDefault() ?? "",
                    IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow
                });
            }

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

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError(nameof(model.Email), "Email này đã tồn tại.");
                return View(model);
            }

            var user = new ApplicationUser
            {
                FullName = model.FullName,
                Email = model.Email.Trim(),
                UserName = model.Email.Trim(),
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);

                try
                {
                    await SendAccountCreatedEmailAsync(user.Email!, user.FullName ?? "", model.Password);
                    TempData["SuccessMessage"] = "Tạo tài khoản thành công và đã gửi email thông tin đăng nhập.";
                }
                catch
                {
                    TempData["ErrorMessage"] = "Tạo tài khoản thành công nhưng gửi email thất bại. Vui lòng kiểm tra cấu hình EmailSettings.";
                }

                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
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
                FullName = user.FullName ?? "",
                Email = user.Email ?? "",
                Role = roles.FirstOrDefault() ?? ""
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

            var duplicatedEmailUser = await _userManager.FindByEmailAsync(model.Email.Trim());
            if (duplicatedEmailUser != null && duplicatedEmailUser.Id != model.Id)
            {
                ModelState.AddModelError(nameof(model.Email), "Email này đã tồn tại.");
                return View(model);
            }

            user.FullName = model.FullName.Trim();
            user.Email = model.Email.Trim();
            user.UserName = model.Email.Trim();

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
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    foreach (var error in removeResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View(model);
                }
            }

            var addResult = await _userManager.AddToRoleAsync(user, model.Role);
            if (!addResult.Succeeded)
            {
                foreach (var error in addResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            TempData["SuccessMessage"] = "Cập nhật tài khoản thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
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
                FullName = user.FullName ?? "",
                Email = user.Email ?? "",
                Role = roles.FirstOrDefault() ?? "",
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
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.Email == User.Identity?.Name)
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
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.Email == User.Identity?.Name)
            {
                TempData["ErrorMessage"] = "Bạn không thể tự khóa tài khoản của chính mình.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.SetLockoutEndDateAsync(
                user,
                DateTimeOffset.UtcNow.AddYears(100));

            if (result.Succeeded)
            {
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
            if (string.IsNullOrEmpty(id))
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

        private async Task SendAccountCreatedEmailAsync(string email, string fullName, string password)
        {
            var subject = "Thông tin tài khoản đăng nhập hệ thống QLKH";

            var body = $@"
            <p>Xin chào <strong>{fullName}</strong>,</p>

            <p>Tài khoản của bạn trên hệ thống QLKH đã được tạo thành công.</p>

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

            <p>Trân trọng,<br />Hệ thống QLKH</p>
            ";

            await _emailSenderService.SendEmailAsync(email, subject, body);
        }
    }
}