using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Domain.Entities;

namespace QLKH.Application.Interfaces.Repositories
{
    public interface IStudentCertificateRepository
    {
        Task<IEnumerable<StudentCertificate>> GetAllAsync();
        Task<IEnumerable<StudentCertificate>> GetByStudentIdAsync(int studentId);
        Task<StudentCertificate?> GetByIdAsync(int id);
        Task AddAsync(StudentCertificate certificate);
        Task UpdateAsync(StudentCertificate certificate);
        Task DeleteAsync(StudentCertificate certificate);
        Task SaveChangesAsync();
    }
}