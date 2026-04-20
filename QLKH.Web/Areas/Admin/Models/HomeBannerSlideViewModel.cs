using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace QLKH.Web.Areas.Admin.Models
{
    public class HomeBannerSlideViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Tiêu đề")]
        public string? Title { get; set; }

        [Display(Name = "Ảnh banner")]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Ảnh hiện tại")]
        public string? ExistingImageUrl { get; set; }

        [Display(Name = "Mô tả ảnh")]
        public string? AltText { get; set; }

        [Display(Name = "Đang hiển thị")]
        public bool IsActive { get; set; } = true;

        public int DisplayOrder { get; set; }
    }
}