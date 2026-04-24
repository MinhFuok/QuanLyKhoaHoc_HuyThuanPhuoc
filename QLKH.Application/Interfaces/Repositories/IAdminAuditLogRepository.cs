using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Domain.Entities;

namespace QLKH.Application.Interfaces.Repositories
{
    public interface IAdminAuditLogRepository
    {
        Task AddAsync(AdminAuditLog log);
        Task<List<AdminAuditLog>> GetAllAsync();
        Task<List<string>> GetDistinctActorEmailsAsync();
        Task DeleteOlderThanAsync(DateTime cutoffUtc);
    }
}