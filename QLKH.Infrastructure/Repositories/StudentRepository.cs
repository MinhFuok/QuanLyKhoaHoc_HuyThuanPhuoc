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
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;

        public StudentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _context.Students
                .OrderBy(x => x.Id)
                .ToListAsync();
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            return await _context.Students.FindAsync(id);
        }
        public async Task<Student?> GetByApplicationUserIdAsync(string applicationUserId)
        {
            return await _context.Students
                .FirstOrDefaultAsync(x => x.ApplicationUserId == applicationUserId);
        }
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Students.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> ExistsByCodeAsync(string studentCode)
        {
            return await _context.Students.AnyAsync(x => x.StudentCode == studentCode);
        }
        public async Task<bool> ExistsByApplicationUserIdAsync(string applicationUserId, int? excludeStudentId = null)
        {
            return await _context.Students.AnyAsync(x =>
                x.ApplicationUserId == applicationUserId &&
                (!excludeStudentId.HasValue || x.Id != excludeStudentId.Value));
        }
        public async Task AddAsync(Student student)
        {
            await _context.Students.AddAsync(student);
        }

        public void Update(Student student)
        {
            _context.Students.Update(student);
        }

        public void Delete(Student student)
        {
            _context.Students.Remove(student);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}