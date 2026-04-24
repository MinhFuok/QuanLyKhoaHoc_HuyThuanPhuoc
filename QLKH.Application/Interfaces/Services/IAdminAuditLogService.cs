using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Domain.Entities;

namespace QLKH.Application.Interfaces.Services
{
    public interface IAdminAuditLogService
    {
        Task WriteAsync(
            string? actorUserId,
            string actorEmail,
            string actionName,
            string targetType,
            string? targetId,
            string targetDisplay,
            string? note = null);

        Task<List<AdminAuditLog>> GetAllAsync();
    }
}