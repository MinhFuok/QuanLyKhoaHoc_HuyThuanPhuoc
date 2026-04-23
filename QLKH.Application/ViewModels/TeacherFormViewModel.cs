using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace QLKH.Application.ViewModels
{
    public class TeacherFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã giáo viên.")]
        [Display(Name = "Mã giáo viên")]
        public string TeacherCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn tài khoản liên kết.")]
        [Display(Name = "Tài khoản liên kết")]
        public string ApplicationUserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
        [Display(Name = "Họ tên")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải gồm đúng 10 chữ số.")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Display(Name = "Chuyên môn")]
        public string? Specialization { get; set; }

        public string? LinkedAccountEmail { get; set; }
        public string? LinkedAccountFullName { get; set; }
        public bool LinkedAccountLocked { get; set; }
    }
}