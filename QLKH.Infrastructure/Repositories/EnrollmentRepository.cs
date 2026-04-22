using Microsoft.EntityFrameworkCore;
using QLKH.Application.Interfaces.Repositories;
using QLKH.Domain.Entities;
using QLKH.Infrastructure.Data;

namespace QLKH.Infrastructure.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly AppDbContext _context;

        public EnrollmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Enrollment>> GetAllAsync()
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.ClassRoom!)
                    .ThenInclude(c => c.Course)
                .Include(e => e.ClassRoom!)
                    .ThenInclude(c => c.Teacher)
                .ToListAsync();
        }

        public async Task<Enrollment?> GetByIdAsync(int id)
        {
            return await _context.Enrollments.FindAsync(id);
        }

        public async Task<Enrollment?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.ClassRoom!)
                    .ThenInclude(c => c.Course)
                .Include(e => e.ClassRoom!)
                    .ThenInclude(c => c.Teacher)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Enrollment>> GetByStudentIdAsync(int studentId)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.ClassRoom!)
                    .ThenInclude(c => c.Course)
                .Include(e => e.ClassRoom!)
                    .ThenInclude(c => c.Teacher)
                .Where(e => e.StudentId == studentId)
                .ToListAsync();
        }

        public async Task AddAsync(Enrollment enrollment)
        {
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Enrollment enrollment)
        {
            _context.Enrollments.Update(enrollment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Enrollment enrollment)
        {
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
        }
    }
}