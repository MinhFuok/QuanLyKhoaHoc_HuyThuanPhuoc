using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}