using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Domain.Entities;
using QLKH.Domain.Enums;

namespace QLKH.Application.Interfaces.Services
{
    public interface IEnrollmentService
    {
        Task<IEnumerable<Enrollment>> GetAllAsync();
        Task<Enrollment?> GetByIdAsync(int id);
        Task<Enrollment?> GetByIdWithDetailsAsync(int id);
        Task<bool> CreateAsync(Enrollment enrollment);
        Task<bool> UpdateAsync(Enrollment enrollment);
        Task<bool> DeleteAsync(int id);

        Task<bool> ChangeStatusAsync(int enrollmentId, EnrollmentStatus newStatus);
    }
}