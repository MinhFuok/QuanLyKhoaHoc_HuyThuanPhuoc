using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Application.Interfaces.Repositories;
using QLKH.Application.Interfaces.Services;
using QLKH.Domain.Entities;

namespace QLKH.Application.Services
{
    public class StudentCertificateService : IStudentCertificateService
    {
        private readonly IStudentCertificateRepository _studentCertificateRepository;

        public StudentCertificateService(IStudentCertificateRepository studentCertificateRepository)
        {
            _studentCertificateRepository = studentCertificateRepository;
        }

        public async Task<IEnumerable<StudentCertificate>> GetAllAsync()
        {
            return await _studentCertificateRepository.GetAllAsync();
        }

        public async Task<IEnumerable<StudentCertificate>> GetByStudentIdAsync(int studentId)
        {
            return await _studentCertificateRepository.GetByStudentIdAsync(studentId);
        }

        public async Task<StudentCertificate?> GetByIdAsync(int id)
        {
            return await _studentCertificateRepository.GetByIdAsync(id);
        }

        public async Task CreateAsync(StudentCertificate certificate)
        {
            await _studentCertificateRepository.AddAsync(certificate);
            await _studentCertificateRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(StudentCertificate certificate)
        {
            await _studentCertificateRepository.UpdateAsync(certificate);
            await _studentCertificateRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var certificate = await _studentCertificateRepository.GetByIdAsync(id);
            if (certificate == null)
            {
                return;
            }

            await _studentCertificateRepository.DeleteAsync(certificate);
            await _studentCertificateRepository.SaveChangesAsync();
        }
    }
}