using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKH.Domain.Entities
{
    public class AdminAuditLog
    {
        public int Id { get; set; }

        public string? ActorUserId { get; set; }
        public string ActorEmail { get; set; } = string.Empty;

        public string ActionName { get; set; } = string.Empty;
        public string TargetType { get; set; } = string.Empty;
        public string? TargetId { get; set; }
        public string TargetDisplay { get; set; } = string.Empty;

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}