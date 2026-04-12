using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Domain.Entities;

namespace QLKH.Application.Interfaces.Repositories
{
    public interface IEnrollmentRepository
    {
        Task<IEnumerable<Enrollment>> GetAllAsync();
        Task<Enrollment?> GetByIdAsync(int id);
        Task<IEnumerable<Enrollment>> GetByStudentIdAsync(int studentId);
        Task<Enrollment?> GetByIdWithDetailsAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> IsStudentEnrolledInClassAsync(int studentId, int classRoomId);
        Task AddAsync(Enrollment enrollment);
        void Update(Enrollment enrollment);
        void Delete(Enrollment enrollment);
        Task<int> CountByClassRoomIdAsync(int classRoomId);
        Task SaveChangesAsync();
    }
}