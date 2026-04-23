using Microsoft.EntityFrameworkCore;
using QLKH.Application.Interfaces.Repositories;
using QLKH.Application.ViewModels;
using QLKH.Domain.Entities;
using QLKH.Infrastructure.Data;

namespace QLKH.Infrastructure.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly AppDbContext _context;

        public CourseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _context.Courses
                .OrderBy(x => x.Id)
                .ToListAsync();
        }

        public async Task<Course?> GetByIdAsync(int id)
        {
            return await _context.Courses.FindAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Courses.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> ExistsByCodeAsync(string courseCode)
        {
            return await _context.Courses.AnyAsync(x => x.CourseCode == courseCode);
        }

        public async Task<CourseDeleteImpactViewModel?> GetDeleteImpactAsync(int id)
        {
            var course = await _context.Courses
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (course == null)
            {
                return null;
            }

            var classRoomIds = await _context.ClassRooms
                .Where(x => x.CourseId == id)
                .Select(x => x.Id)
                .ToListAsync();

            var assignmentIds = await _context.Assignments
                .Where(x => classRoomIds.Contains(x.ClassRoomId))
                .Select(x => x.Id)
                .ToListAsync();

            return new CourseDeleteImpactViewModel
            {
                CourseId = course.Id,
                CourseCode = course.CourseCode,
                CourseName = course.CourseName,
                ClassRoomCount = classRoomIds.Count,
                EnrollmentCount = await _context.Enrollments.CountAsync(x => classRoomIds.Contains(x.ClassRoomId)),
                ScheduleCount = await _context.ClassSchedules.CountAsync(x => classRoomIds.Contains(x.ClassRoomId)),
                MaterialCount = await _context.ClassMaterials.CountAsync(x => classRoomIds.Contains(x.ClassRoomId)),
                AssignmentCount = assignmentIds.Count,
                SubmissionCount = await _context.Submissions.CountAsync(x => assignmentIds.Contains(x.AssignmentId)),
                ReviewCount = await _context.TeacherReviews.CountAsync(x => classRoomIds.Contains(x.ClassRoomId))
            };
        }

        public async Task DeleteCascadeAsync(int id)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(x => x.Id == id);
            if (course == null)
            {
                return;
            }

            var classRooms = await _context.ClassRooms
                .Where(x => x.CourseId == id)
                .ToListAsync();

            var classRoomIds = classRooms.Select(x => x.Id).ToList();

            if (classRoomIds.Any())
            {
                var enrollments = await _context.Enrollments
                    .Where(x => classRoomIds.Contains(x.ClassRoomId))
                    .ToListAsync();

                if (enrollments.Any())
                {
                    _context.Enrollments.RemoveRange(enrollments);
                }

                // Các bảng còn lại phần lớn cascade từ ClassRoom hoặc Assignment,
                // nhưng remove chủ động vẫn an toàn và dễ kiểm soát hơn.
                var schedules = await _context.ClassSchedules
                    .Where(x => classRoomIds.Contains(x.ClassRoomId))
                    .ToListAsync();

                if (schedules.Any())
                {
                    _context.ClassSchedules.RemoveRange(schedules);
                }

                var materials = await _context.ClassMaterials
                    .Where(x => classRoomIds.Contains(x.ClassRoomId))
                    .ToListAsync();

                if (materials.Any())
                {
                    _context.ClassMaterials.RemoveRange(materials);
                }

                var reviews = await _context.TeacherReviews
                    .Where(x => classRoomIds.Contains(x.ClassRoomId))
                    .ToListAsync();

                if (reviews.Any())
                {
                    _context.TeacherReviews.RemoveRange(reviews);
                }

                var assignments = await _context.Assignments
                    .Where(x => classRoomIds.Contains(x.ClassRoomId))
                    .ToListAsync();

                var assignmentIds = assignments.Select(x => x.Id).ToList();

                if (assignmentIds.Any())
                {
                    var submissions = await _context.Submissions
                        .Where(x => assignmentIds.Contains(x.AssignmentId))
                        .ToListAsync();

                    if (submissions.Any())
                    {
                        _context.Submissions.RemoveRange(submissions);
                    }
                }

                if (assignments.Any())
                {
                    _context.Assignments.RemoveRange(assignments);
                }

                _context.ClassRooms.RemoveRange(classRooms);
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }

        public async Task AddAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
        }

        public void Update(Course course)
        {
            _context.Courses.Update(course);
        }

        public void Delete(Course course)
        {
            _context.Courses.Remove(course);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}