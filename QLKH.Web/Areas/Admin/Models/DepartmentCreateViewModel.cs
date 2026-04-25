using System.ComponentModel.DataAnnotations;

namespace QLKH.Web.Areas.Admin.Models
{
    public class DepartmentCreateViewModel
    {
        [Display(Name = "Mã chương trình/khóa")]
        public string DepartmentCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên")]

        [Display(Name = "Tên chương trình/khóa")]
        public string DepartmentName { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Danh mục cha (để trống nếu là chương trình)")]
        public int? ParentDepartmentId { get; set; }

        [Display(Name = "Đang sử dụng")]
        public bool IsActive { get; set; } = true;
    }
}