using System.ComponentModel.DataAnnotations;

namespace QLKH.Web.Areas.Admin.Models
{
    public class SystemSettingViewModel
    {
        public int Id { get; set; }

        // =========================
        // TAB: GIAO DIỆN WEBSITE
        // =========================
        [Display(Name = "Tên website")]
        public string SiteName { get; set; } = string.Empty;

        [Display(Name = "Tiêu đề chính trang chủ")]
        public string HomeBannerTitle { get; set; } = string.Empty;

        [Display(Name = "Phụ đề trang chủ")]
        public string HomeBannerSubtitle { get; set; } = string.Empty;

        [Display(Name = "Footer text")]
        public string FooterText { get; set; } = string.Empty;

        // =========================
        // TAB: 4 CARD NỔI BẬT
        // =========================
        [Display(Name = "Số thẻ 1")]
        public string? FeatureCard1Number { get; set; }

        [Display(Name = "Tiêu đề thẻ 1")]
        public string? FeatureCard1Title { get; set; }

        [Display(Name = "Mô tả thẻ 1")]
        public string? FeatureCard1Description { get; set; }

        [Display(Name = "Số thẻ 2")]
        public string? FeatureCard2Number { get; set; }

        [Display(Name = "Tiêu đề thẻ 2")]
        public string? FeatureCard2Title { get; set; }

        [Display(Name = "Mô tả thẻ 2")]
        public string? FeatureCard2Description { get; set; }

        [Display(Name = "Số thẻ 3")]
        public string? FeatureCard3Number { get; set; }

        [Display(Name = "Tiêu đề thẻ 3")]
        public string? FeatureCard3Title { get; set; }

        [Display(Name = "Mô tả thẻ 3")]
        public string? FeatureCard3Description { get; set; }

        [Display(Name = "Số thẻ 4")]
        public string? FeatureCard4Number { get; set; }

        [Display(Name = "Tiêu đề thẻ 4")]
        public string? FeatureCard4Title { get; set; }

        [Display(Name = "Mô tả thẻ 4")]
        public string? FeatureCard4Description { get; set; }

        // =========================
        // TAB: LIÊN HỆ
        // =========================
        [Display(Name = "Email liên hệ")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string ContactEmail { get; set; } = string.Empty;

        [Display(Name = "Số điện thoại")]
        public string ContactPhone { get; set; } = string.Empty;

        [Display(Name = "Địa chỉ")]
        public string ContactAddress { get; set; } = string.Empty;

        [Display(Name = "Link Facebook")]
        public string FacebookUrl { get; set; } = string.Empty;

        [Display(Name = "Link YouTube")]
        public string YoutubeUrl { get; set; } = string.Empty;

        [Display(Name = "Link TikTok")]
        public string TiktokUrl { get; set; } = string.Empty;

        [Display(Name = "Link X")]
        public string XUrl { get; set; } = string.Empty;

        // =========================
        // TAB: TRẠNG THÁI WEBSITE
        // =========================
        [Display(Name = "Website đang hoạt động")]
        public bool IsWebsiteEnabled { get; set; }

        [Display(Name = "Thông báo bảo trì")]
        public string MaintenanceMessage { get; set; } = string.Empty;

        // =========================
        // TAB: EMAIL HỆ THỐNG
        // =========================
        [Display(Name = "Bật gửi email")]
        public bool EnableEmail { get; set; }

        [Display(Name = "SMTP Server")]
        public string SmtpServer { get; set; } = string.Empty;

        [Display(Name = "SMTP Port")]
        [Range(1, 65535, ErrorMessage = "SMTP Port không hợp lệ")]
        public int SmtpPort { get; set; } = 587;

        [Display(Name = "Tên người gửi")]
        public string SenderName { get; set; } = string.Empty;

        [Display(Name = "Email người gửi")]
        [EmailAddress(ErrorMessage = "Email người gửi không hợp lệ")]
        public string SenderEmail { get; set; } = string.Empty;

        [Display(Name = "SMTP Username")]
        public string SmtpUsername { get; set; } = string.Empty;

        [Display(Name = "SMTP Password")]
        public string SmtpPassword { get; set; } = string.Empty;

        public string HomeBannerImageUrl { get; set; } = string.Empty;
    }
}