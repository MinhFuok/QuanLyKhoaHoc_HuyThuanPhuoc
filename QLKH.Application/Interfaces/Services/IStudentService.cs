using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Domain.Entities;

namespace QLKH.Application.Interfaces.Services
{
    public interface IStudentService
    {
        Task<IEnumerable<Student>> GetAllAsync();
        Task<Student?> GetByIdAsync(int id);
        Task<Student?> GetByApplicationUserIdAsync(string applicationUserId);
        Task<IEnumerable<ClassRoom>> GetMyClassesAsync(string applicationUserId);
        Task<bool> CreateAsync(Student student);
        Task<bool> UpdateAsync(Student student);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsByApplicationUserIdAsync(string applicationUserId, int? excludeStudentId = null);
        Task<string> GenerateNextStudentCodeAsync();
    }
}