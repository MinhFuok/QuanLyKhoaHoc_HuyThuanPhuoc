using System.ComponentModel.DataAnnotations;

namespace QLKH.Web.Areas.Admin.Models
{
    public class DepartmentCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mã")]
        [Display(Name = "Mã khoa/ngành")]
        public string DepartmentCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên")]
        [Display(Name = "Tên khoa/ngành")]
        public string DepartmentName { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Danh mục cha (để trống nếu là khoa)")]
        public int? ParentDepartmentId { get; set; }

        [Display(Name = "Đang sử dụng")]
        public bool IsActive { get; set; } = true;
    }
}