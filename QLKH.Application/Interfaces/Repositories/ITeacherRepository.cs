using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Domain.Entities;

namespace QLKH.Application.Interfaces.Repositories
{
    public interface ITeacherRepository
    {
        Task<IEnumerable<Teacher>> GetAllAsync();
        Task<Teacher?> GetByIdAsync(int id);
        Task<Teacher?> GetByApplicationUserIdAsync(string applicationUserId);
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsByCodeAsync(string teacherCode);
        Task<bool> ExistsByApplicationUserIdAsync(string applicationUserId, int? excludeTeacherId = null);
        Task<string?> GetLatestTeacherCodeAsync();
        Task AddAsync(Teacher teacher);
        void Update(Teacher teacher);
        void Delete(Teacher teacher);
        Task SaveChangesAsync();
    }
}