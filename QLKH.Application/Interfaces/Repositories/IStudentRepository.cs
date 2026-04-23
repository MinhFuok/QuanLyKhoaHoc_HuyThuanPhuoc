using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Domain.Entities;

namespace QLKH.Application.Interfaces.Repositories
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllAsync();
        Task<Student?> GetByIdAsync(int id);
        Task<Student?> GetByApplicationUserIdAsync(string applicationUserId);
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsByCodeAsync(string studentCode);
        Task<bool> ExistsByApplicationUserIdAsync(string applicationUserId, int? excludeStudentId = null);
        Task AddAsync(Student student);
        void Update(Student student);
        void Delete(Student student);
        Task SaveChangesAsync();
    }
}