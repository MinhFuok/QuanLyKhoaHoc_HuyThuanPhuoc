using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Domain.Entities;

namespace QLKH.Application.Interfaces.Repositories
{
    public interface IClassRoomRepository
    {
        Task<IEnumerable<ClassRoom>> GetAllAsync();
        Task<ClassRoom?> GetByIdAsync(int id);
        Task<IEnumerable<ClassRoom>> GetByTeacherIdAsync(int teacherId);
        Task<ClassRoom?> GetByIdWithDetailsAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsByCodeAsync(string classCode);
        Task AddAsync(ClassRoom classRoom);
        void Update(ClassRoom classRoom);
        void Delete(ClassRoom classRoom);
        Task SaveChangesAsync();
    }
}