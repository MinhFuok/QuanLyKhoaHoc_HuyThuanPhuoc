using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace QLKH.Web.Areas.Admin.Models
{
    public class BulkNotificationViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề.")]
        [Display(Name = "Tiêu đề")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập nội dung.")]
        [Display(Name = "Nội dung")]
        public string Message { get; set; } = string.Empty;

        [Display(Name = "Gửi cho học viên")]
        public bool SendToStudents { get; set; }

        [Display(Name = "Gửi cho giáo viên")]
        public bool SendToTeachers { get; set; }

        [Display(Name = "Tệp đính kèm")]
        public IFormFile? AttachmentFile { get; set; }

        public int SentCount { get; set; }
        public int FailedCount { get; set; }
    }
}