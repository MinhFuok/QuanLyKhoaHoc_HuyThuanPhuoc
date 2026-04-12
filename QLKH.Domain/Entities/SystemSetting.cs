namespace QLKH.Domain.Entities
{
    public class SystemSetting
    {
        public int Id { get; set; }

        public string SiteName { get; set; } = "QLKH";
        public string? HomeBannerTitle { get; set; }
        public string? HomeBannerSubtitle { get; set; }
        public string? HomeBannerImageUrl { get; set; }

        public string? FooterText { get; set; }

        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactAddress { get; set; }

        public string? FacebookUrl { get; set; }
        public string? YoutubeUrl { get; set; }
        public string? TiktokUrl { get; set; }
        public string? XUrl { get; set; }
        public bool IsWebsiteEnabled { get; set; } = true;
        public string? MaintenanceMessage { get; set; }

        public bool EnableEmail { get; set; } = true;
        public string? SmtpServer { get; set; }
        public int? SmtpPort { get; set; }
        public string? SenderName { get; set; }
        public string? SenderEmail { get; set; }
        public string? SmtpUsername { get; set; }
        public string? SmtpPassword { get; set; }

        public bool EnableVnPay { get; set; } = false;
        public bool EnableMomo { get; set; } = false;
    }
}