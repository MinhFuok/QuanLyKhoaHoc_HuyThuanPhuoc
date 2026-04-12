using Microsoft.EntityFrameworkCore;
using QLKH.Application.Interfaces.Repositories;
using QLKH.Domain.Entities;
using QLKH.Infrastructure.Data;

namespace QLKH.Infrastructure.Repositories
{
    public class TeacherReviewRepository : ITeacherReviewRepository
    {
        private readonly AppDbContext _context;

        public TeacherReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TeacherReview>> GetAllAsync()
        {
            return await _context.TeacherReviews
                .Include(x => x.Teacher)
                .Include(x => x.Student)
                .Include(x => x.ClassRoom)
                    .ThenInclude(x => x.Course)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<TeacherReview?> GetByIdAsync(int id)
        {
            return await _context.TeacherReviews
                .Include(x => x.Teacher)
                .Include(x => x.Student)
                .Include(x => x.ClassRoom)
                    .ThenInclude(x => x.Course)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<TeacherReview?> GetByStudentAndClassRoomAsync(int studentId, int classRoomId)
        {
            return await _context.TeacherReviews
                .Include(x => x.Teacher)
                .Include(x => x.Student)
                .Include(x => x.ClassRoom)
                    .ThenInclude(x => x.Course)
                .FirstOrDefaultAsync(x => x.StudentId == studentId && x.ClassRoomId == classRoomId);
        }

        public async Task<IEnumerable<TeacherReview>> GetByTeacherIdAsync(int teacherId)
        {
            return await _context.TeacherReviews
                .Include(x => x.Teacher)
                .Include(x => x.Student)
                .Include(x => x.ClassRoom)
                    .ThenInclude(x => x.Course)
                .Where(x => x.TeacherId == teacherId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TeacherReview>> GetByClassRoomIdAsync(int classRoomId)
        {
            return await _context.TeacherReviews
                .Include(x => x.Teacher)
                .Include(x => x.Student)
                .Include(x => x.ClassRoom)
                    .ThenInclude(x => x.Course)
                .Where(x => x.ClassRoomId == classRoomId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(TeacherReview teacherReview)
        {
            await _context.TeacherReviews.AddAsync(teacherReview);
        }

        public void Update(TeacherReview teacherReview)
        {
            _context.TeacherReviews.Update(teacherReview);
        }

        public void Delete(TeacherReview teacherReview)
        {
            _context.TeacherReviews.Remove(teacherReview);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}