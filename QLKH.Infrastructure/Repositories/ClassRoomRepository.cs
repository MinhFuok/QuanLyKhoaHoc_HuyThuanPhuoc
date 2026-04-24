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
    public class ClassRoomRepository : IClassRoomRepository
    {
        private readonly AppDbContext _context;

        public ClassRoomRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClassRoom>> GetAllAsync()
        {
            return await _context.ClassRooms
                .Include(x => x.Course)
                .Include(x => x.Teacher)
                .OrderBy(x => x.Id)
                .ToListAsync();
        }

        public async Task<ClassRoom?> GetByIdAsync(int id)
        {
            return await _context.ClassRooms.FindAsync(id);
        }
        public async Task<IEnumerable<ClassRoom>> GetByTeacherIdAsync(int teacherId)
        {
            return await _context.ClassRooms
                .Include(x => x.Course)
                .Include(x => x.Teacher)
                .Where(x => x.TeacherId == teacherId)
                .ToListAsync();
        }
        public async Task<ClassRoom?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.ClassRooms
                .Include(x => x.Course)
                .Include(x => x.Teacher)
                .Include(x => x.Enrollments)
                    .ThenInclude(x => x.Student)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.ClassRooms.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> ExistsByCodeAsync(string classCode)
        {
            return await _context.ClassRooms.AnyAsync(x => x.ClassCode == classCode);
        }

        public async Task AddAsync(ClassRoom classRoom)
        {
            await _context.ClassRooms.AddAsync(classRoom);
        }

        public void Update(ClassRoom classRoom)
        {
            _context.ClassRooms.Update(classRoom);
        }

        public void Delete(ClassRoom classRoom)
        {
            _context.ClassRooms.Remove(classRoom);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}