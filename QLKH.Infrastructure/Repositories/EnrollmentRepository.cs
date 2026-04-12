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
                .Include(x => x.Student)
                .Include(x => x.ClassRoom)
                    .ThenInclude(x => x!.Course)
                .Include(x => x.ClassRoom)
                    .ThenInclude(x => x!.Teacher)
                .OrderByDescending(x => x.EnrolledAt)
                .ToListAsync();
        }

        public async Task<Enrollment?> GetByIdAsync(int id)
        {
            return await _context.Enrollments.FindAsync(id);
        }
        public async Task<IEnumerable<Enrollment>> GetByStudentIdAsync(int studentId)
        {
            return await _context.Enrollments
                .Include(x => x.Student)
                .Include(x => x.ClassRoom)
                    .ThenInclude(x => x.Course)
                .Include(x => x.ClassRoom)
                    .ThenInclude(x => x.Teacher)
                .Where(x => x.StudentId == studentId)
                .ToListAsync();
        }
        public async Task<Enrollment?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Enrollments
                .Include(x => x.Student)
                .Include(x => x.ClassRoom)
                    .ThenInclude(x => x!.Course)
                .Include(x => x.ClassRoom)
                    .ThenInclude(x => x!.Teacher)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Enrollments.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> IsStudentEnrolledInClassAsync(int studentId, int classRoomId)
        {
            return await _context.Enrollments.AnyAsync(x => x.StudentId == studentId && x.ClassRoomId == classRoomId);
        }

        public async Task AddAsync(Enrollment enrollment)
        {
            await _context.Enrollments.AddAsync(enrollment);
        }

        public void Update(Enrollment enrollment)
        {
            _context.Enrollments.Update(enrollment);
        }

        public void Delete(Enrollment enrollment)
        {
            _context.Enrollments.Remove(enrollment);
        }

        public async Task<int> CountByClassRoomIdAsync(int classRoomId)
        {
            return await _context.Enrollments.CountAsync(x => x.ClassRoomId == classRoomId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}