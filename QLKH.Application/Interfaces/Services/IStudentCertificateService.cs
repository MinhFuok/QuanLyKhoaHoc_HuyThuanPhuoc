using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Domain.Entities;

namespace QLKH.Application.Interfaces.Services
{
    public interface IStudentCertificateService
    {
        Task<IEnumerable<StudentCertificate>> GetAllAsync();
        Task<IEnumerable<StudentCertificate>> GetByStudentIdAsync(int studentId);
        Task<StudentCertificate?> GetByIdAsync(int id);
        Task CreateAsync(StudentCertificate certificate);
        Task UpdateAsync(StudentCertificate certificate);
        Task DeleteAsync(int id);
    }
}