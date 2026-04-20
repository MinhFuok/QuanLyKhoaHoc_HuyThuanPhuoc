using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QLKH.Application.Interfaces.Repositories;
using QLKH.Domain.Entities;
using QLKH.Infrastructure.Data;

namespace QLKH.Infrastructure.Repositories
{
    public class StudentCertificateRepository : IStudentCertificateRepository
    {
        private readonly AppDbContext _context;

        public StudentCertificateRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StudentCertificate>> GetAllAsync()
        {
            return await _context.StudentCertificates
                .Include(x => x.Student)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentCertificate>> GetByStudentIdAsync(int studentId)
        {
            return await _context.StudentCertificates
                .Include(x => x.Student)
                .Where(x => x.StudentId == studentId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<StudentCertificate?> GetByIdAsync(int id)
        {
            return await _context.StudentCertificates
                .Include(x => x.Student)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(StudentCertificate certificate)
        {
            await _context.StudentCertificates.AddAsync(certificate);
        }

        public Task UpdateAsync(StudentCertificate certificate)
        {
            _context.StudentCertificates.Update(certificate);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(StudentCertificate certificate)
        {
            _context.StudentCertificates.Remove(certificate);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}