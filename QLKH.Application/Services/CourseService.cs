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
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;

        public CourseService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _courseRepository.GetAllAsync();
        }

        public async Task<Course?> GetByIdAsync(int id)
        {
            return await _courseRepository.GetByIdAsync(id);
        }

        public async Task<bool> CreateAsync(Course course)
        {
            if (await _courseRepository.ExistsByCodeAsync(course.CourseCode))
            {
                return false;
            }

            await _courseRepository.AddAsync(course);
            await _courseRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(Course course)
        {
            var existingCourse = await _courseRepository.GetByIdAsync(course.Id);
            if (existingCourse == null)
            {
                return false;
            }

            existingCourse.CourseCode = course.CourseCode;
            existingCourse.CourseName = course.CourseName;
            existingCourse.Description = course.Description;
            existingCourse.DurationInMonths = course.DurationInMonths;
            existingCourse.Fee = course.Fee;
            existingCourse.Status = course.Status;

            _courseRepository.Update(existingCourse);
            await _courseRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existingCourse = await _courseRepository.GetByIdAsync(id);
            if (existingCourse == null)
            {
                return false;
            }

            _courseRepository.Delete(existingCourse);
            await _courseRepository.SaveChangesAsync();
            return true;
        }
    }
}
