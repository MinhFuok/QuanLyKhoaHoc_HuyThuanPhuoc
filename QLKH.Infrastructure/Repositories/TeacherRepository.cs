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
    public class TeacherRepository : ITeacherRepository
    {
        private readonly AppDbContext _context;

        public TeacherRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Teacher>> GetAllAsync()
        {
            return await _context.Teachers
                .OrderBy(x => x.Id)
                .ToListAsync();
        }

        public async Task<Teacher?> GetByIdAsync(int id)
        {
            return await _context.Teachers.FindAsync(id);
        }
        public async Task<Teacher?> GetByApplicationUserIdAsync(string applicationUserId)
        {
            return await _context.Teachers
                .FirstOrDefaultAsync(x => x.ApplicationUserId == applicationUserId);
        }
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Teachers.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> ExistsByCodeAsync(string teacherCode)
        {
            return await _context.Teachers.AnyAsync(x => x.TeacherCode == teacherCode);
        }
        public async Task<bool> ExistsByApplicationUserIdAsync(string applicationUserId, int? excludeTeacherId = null)
        {
            return await _context.Teachers.AnyAsync(x =>
                x.ApplicationUserId == applicationUserId &&
                (!excludeTeacherId.HasValue || x.Id != excludeTeacherId.Value));
        }
        public async Task<string?> GetLatestTeacherCodeAsync()
        {
            return await _context.Teachers
                .OrderByDescending(x => x.Id)
                .Select(x => x.TeacherCode)
                .FirstOrDefaultAsync();
        }
        public async Task AddAsync(Teacher teacher)
        {
            await _context.Teachers.AddAsync(teacher);
        }

        public void Update(Teacher teacher)
        {
            _context.Teachers.Update(teacher);
        }

        public void Delete(Teacher teacher)
        {
            _context.Teachers.Remove(teacher);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}