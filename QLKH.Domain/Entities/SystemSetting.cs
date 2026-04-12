using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKH.Domain.Entities
{
    public class SystemSetting
    {
        public int Id { get; set; }

        public string SiteName { get; set; } = "QLKH";
        public string? HomeBannerTitle { get; set; }
        public string? HomeBannerImageUrl { get; set; }

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