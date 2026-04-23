using Microsoft.EntityFrameworkCore;
using QLKH.Application.Interfaces.Repositories;
using QLKH.Domain.Entities;
using QLKH.Infrastructure.Data;

namespace QLKH.Infrastructure.Repositories
{
    public class ClassScheduleRepository : IClassScheduleRepository
    {
        private readonly AppDbContext _context;

        public ClassScheduleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClassSchedule>> GetAllAsync()
        {
            return await _context.ClassSchedules
                .Include(x => x.ClassRoom)
                    .ThenInclude(x => x.Course)
                .Include(x => x.ClassRoom)
                    .ThenInclude(x => x.Teacher)
                .OrderBy(x => x.StudyDate)
                .ThenBy(x => x.StartTime)
                .ToListAsync();
        }

        public async Task<ClassSchedule?> GetByIdAsync(int id)
        {
            return await _context.ClassSchedules
                .Include(x => x.ClassRoom)
                    .ThenInclude(x => x.Course)
                .Include(x => x.ClassRoom)
                    .ThenInclude(x => x.Teacher)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<ClassSchedule>> GetByClassRoomIdAsync(int classRoomId)
        {
            return await _context.ClassSchedules
                .Include(x => x.ClassRoom)
                    .ThenInclude(x => x.Course)
                .Include(x => x.ClassRoom)
                    .ThenInclude(x => x.Teacher)
                .Where(x => x.ClassRoomId == classRoomId)
                .OrderBy(x => x.StudyDate)
                .ThenBy(x => x.StartTime)
                .ToListAsync();
        }

        public async Task AddAsync(ClassSchedule classSchedule)
        {
            await _context.ClassSchedules.AddAsync(classSchedule);
        }

        public void Update(ClassSchedule classSchedule)
        {
            _context.ClassSchedules.Update(classSchedule);
        }

        public void Delete(ClassSchedule classSchedule)
        {
            _context.ClassSchedules.Remove(classSchedule);
        }

        public async Task<bool> ExistsConflictAsync(
            DateTime studyDate,
            TimeSpan startTime,
            TimeSpan endTime,
            int? excludeId = null)
        {
            var date = studyDate.Date;

            return await _context.ClassSchedules.AnyAsync(x =>
                x.StudyDate.Date == date &&
                x.StartTime < endTime &&
                startTime < x.EndTime &&
                (!excludeId.HasValue || x.Id != excludeId.Value));
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}