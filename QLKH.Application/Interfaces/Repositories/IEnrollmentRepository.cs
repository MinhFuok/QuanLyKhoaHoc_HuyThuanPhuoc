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
        Task<Enrollment?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Enrollment>> GetByStudentIdAsync(int studentId);
        Task AddAsync(Enrollment enrollment);

        Task UpdateAsync(Enrollment enrollment);
        Task DeleteAsync(Enrollment enrollment);

    }
}