using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Application.Interfaces.Repositories;
using QLKH.Application.Interfaces.Services;
using QLKH.Domain.Entities;

namespace QLKH.Application.Services
{
    public class AdminAuditLogService : IAdminAuditLogService
    {
        private readonly IAdminAuditLogRepository _repository;

        public AdminAuditLogService(IAdminAuditLogRepository repository)
        {
            _repository = repository;
        }

        public async Task WriteAsync(
            string? actorUserId,
            string actorEmail,
            string actionName,
            string targetType,
            string? targetId,
            string targetDisplay,
            string? note = null)
        {
            await CleanupOldLogsAsync();

            var log = new AdminAuditLog
            {
                ActorUserId = actorUserId,
                ActorEmail = actorEmail,
                ActionName = actionName,
                TargetType = targetType,
                TargetId = targetId,
                TargetDisplay = targetDisplay,
                Note = note,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(log);
        }

        public async Task<List<AdminAuditLog>> GetAllAsync()
        {
            await CleanupOldLogsAsync();
            return await _repository.GetAllAsync();
        }

        public async Task<List<string>> GetDistinctActorEmailsAsync()
        {
            await CleanupOldLogsAsync();
            return await _repository.GetDistinctActorEmailsAsync();
        }

        public async Task CleanupOldLogsAsync()
        {
            var cutoffUtc = DateTime.UtcNow.AddDays(-30);
            await _repository.DeleteOlderThanAsync(cutoffUtc);
        }
    }
}