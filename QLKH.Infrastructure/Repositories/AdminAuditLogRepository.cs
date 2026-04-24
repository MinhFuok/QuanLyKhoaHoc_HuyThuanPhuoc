using Microsoft.EntityFrameworkCore;
using QLKH.Application.Interfaces.Repositories;
using QLKH.Domain.Entities;
using QLKH.Infrastructure.Data;

namespace QLKH.Infrastructure.Repositories
{
    public class AdminAuditLogRepository : IAdminAuditLogRepository
    {
        private readonly AppDbContext _context;

        public AdminAuditLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AdminAuditLog log)
        {
            await _context.AdminAuditLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AdminAuditLog>> GetAllAsync()
        {
            return await _context.AdminAuditLogs
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<string>> GetDistinctActorEmailsAsync()
        {
            return await _context.AdminAuditLogs
                .Where(x => !string.IsNullOrWhiteSpace(x.ActorEmail))
                .Select(x => x.ActorEmail)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();
        }

        public async Task DeleteOlderThanAsync(DateTime cutoffUtc)
        {
            var oldLogs = await _context.AdminAuditLogs
                .Where(x => x.CreatedAt < cutoffUtc)
                .ToListAsync();

            if (oldLogs.Any())
            {
                _context.AdminAuditLogs.RemoveRange(oldLogs);
                await _context.SaveChangesAsync();
            }
        }
    }
}