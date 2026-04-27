using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QLKH.Web.Areas.HocVu.Models
{
    public class StudentCertificateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn học viên")]
        [Display(Name = "Học viên")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên chứng chỉ")]
        [Display(Name = "Tên chứng chỉ")]
        public string CertificateName { get; set; } = string.Empty;

        [Display(Name = "Mã/Số chứng chỉ")]
        public string? CertificateCode { get; set; }

        [Display(Name = "Ngày cấp")]
        [DataType(DataType.Date)]
        public DateTime? IssuedDate { get; set; }
        [Display(Name = "Ngày hết hạn")]
        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }

        [Display(Name = "Đơn vị cấp")]
        public string? IssuedBy { get; set; }

        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }

        [Display(Name = "File minh chứng")]
        public IFormFile? EvidenceFile { get; set; }

        public string? ExistingEvidenceFilePath { get; set; }

        [Display(Name = "Đã xác nhận")]
        public bool IsApproved { get; set; } = true;

        public List<SelectListItem> StudentOptions { get; set; } = new();
    }
}