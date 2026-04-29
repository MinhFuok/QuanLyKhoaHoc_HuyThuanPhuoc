using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKH.Application.Messages
{
    public class AdminNotificationMessage
    {
        public string Subject { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string TargetText { get; set; } = string.Empty;

        public int SentCount { get; set; }

        public int FailedCount { get; set; }

        public string? AttachmentFileName { get; set; }

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}