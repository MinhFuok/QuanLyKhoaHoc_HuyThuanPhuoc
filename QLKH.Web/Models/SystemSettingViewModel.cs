using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace QLKH.Web.Areas.Admin.Models
{
    public class SystemSettingViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Tên website")]
        public string SiteName { get; set; } = "QLKH";

        [Display(Name = "Tiêu đề banner trang chủ")]
        public string? HomeBannerTitle { get; set; }

        public string? HomeBannerImageUrl { get; set; }

        [Display(Name = "Ảnh banner")]
        public IFormFile? BannerImageFile { get; set; }

        [Display(Name = "Bật gửi email")]
        public bool EnableEmail { get; set; } = true;

        [Display(Name = "SMTP Server")]
        public string? SmtpServer { get; set; }

        [Display(Name = "SMTP Port")]
        public int? SmtpPort { get; set; }

        [Display(Name = "Tên người gửi")]
        public string? SenderName { get; set; }

        [Display(Name = "Email người gửi")]
        public string? SenderEmail { get; set; }

        [Display(Name = "SMTP Username")]
        public string? SmtpUsername { get; set; }

        [Display(Name = "SMTP Password")]
        public string? SmtpPassword { get; set; }

        [Display(Name = "Bật VNPay")]
        public bool EnableVnPay { get; set; }

        [Display(Name = "Bật Momo")]
        public bool EnableMomo { get; set; }
    }
}