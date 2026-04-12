using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QLKH.Infrastructure.Data;

namespace QLKH.Infrastructure.Identity
{
    public static class IdentitySeedData
    {
        public static async Task SeedAsync(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            AppDbContext context)
        {
            string[] roles = { "Admin", "HocVu", "GiaoVien", "HocVien" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            // acc test
            var testStudentEmail = "nsmp2022@gmail.com";
            var testStudentUser = await userManager.FindByEmailAsync(testStudentEmail);

            if (testStudentUser == null)
            {
                testStudentUser = new ApplicationUser
                {
                    UserName = testStudentEmail,
                    Email = testStudentEmail,
                    FullName = "Hoc Vien Test Gmail",
                    EmailConfirmed = true
                };

                var createTestStudentResult = await userManager.CreateAsync(testStudentUser, "P@ssw0rd");

                if (createTestStudentResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(testStudentUser, "HocVien");
                }
            }
            var adminUser = await CreateUserAsync(
                userManager,
                "admin@qlkh.com",
                "P@ssw0rd",
                "Quản trị viên",
                "Admin");

            var hocVuUser = await CreateUserAsync(
                userManager,
                "hocvu@qlkh.com",
                "P@ssw0rd",
                "Nhân viên học vụ",
                "HocVu");

            var giaoVienUser = await CreateUserAsync(
                userManager,
                "giaovien@qlkh.com",
                "P@ssw0rd",
                "Giáo viên mẫu",
                "GiaoVien");

            var hocVienUser = await CreateUserAsync(
                userManager,
                "hocvien@qlkh.com",
                "P@ssw0rd",
                "Học viên mẫu",
                "HocVien");

            // Map Teacher mẫu với tài khoản giáo viên
            var teacher = await context.Teachers.FirstOrDefaultAsync(x => x.TeacherCode == "GV001");
            if (teacher != null)
            {
                teacher.Email = "giaovien@qlkh.com";
                teacher.ApplicationUserId = giaoVienUser.Id;
            }

            // Map Student mẫu với tài khoản học viên
            var student = await context.Students.FirstOrDefaultAsync(x => x.StudentCode == "HV001");
            if (student != null)
            {
                student.Email = "hocvien@qlkh.com";
                student.ApplicationUserId = hocVienUser.Id;
            }

            await context.SaveChangesAsync();
        }

        private static async Task<ApplicationUser> CreateUserAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string password,
            string fullName,
            string role)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Không tạo được user {email}: {errors}");
                }
            }

            if (!await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
            }

            return user;
        }
    }
}