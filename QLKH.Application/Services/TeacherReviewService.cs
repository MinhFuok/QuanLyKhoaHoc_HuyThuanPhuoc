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
    public class TeacherReviewService : ITeacherReviewService
    {
        private readonly ITeacherReviewRepository _teacherReviewRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IClassRoomRepository _classRoomRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;

        public TeacherReviewService(
            ITeacherReviewRepository teacherReviewRepository,
            IStudentRepository studentRepository,
            ITeacherRepository teacherRepository,
            IClassRoomRepository classRoomRepository,
            IEnrollmentRepository enrollmentRepository)
        {
            _teacherReviewRepository = teacherReviewRepository;
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
            _classRoomRepository = classRoomRepository;
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<IEnumerable<TeacherReview>> GetAllAsync()
        {
            return await _teacherReviewRepository.GetAllAsync();
        }

        public async Task<TeacherReview?> GetByIdAsync(int id)
        {
            return await _teacherReviewRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<TeacherReview>> GetMyTeacherReviewsAsync(string applicationUserId)
        {
            var teacher = await _teacherRepository.GetByApplicationUserIdAsync(applicationUserId);
            if (teacher == null)
            {
                return Enumerable.Empty<TeacherReview>();
            }

            return await _teacherReviewRepository.GetByTeacherIdAsync(teacher.Id);
        }

        public async Task<TeacherReview?> GetMyReviewForClassAsync(string applicationUserId, int classRoomId)
        {
            var student = await _studentRepository.GetByApplicationUserIdAsync(applicationUserId);
            if (student == null)
            {
                return null;
            }

            return await _teacherReviewRepository.GetByStudentAndClassRoomAsync(student.Id, classRoomId);
        }

        public async Task<bool> SubmitReviewAsync(string applicationUserId, int classRoomId, int rating, string? comment)
        {
            var student = await _studentRepository.GetByApplicationUserIdAsync(applicationUserId);
            if (student == null)
            {
                return false;
            }

            var classRoom = await _classRoomRepository.GetByIdAsync(classRoomId);
            if (classRoom == null)
            {
                return false;
            }

            var enrollments = await _enrollmentRepository.GetByStudentIdAsync(student.Id);
            var isMyClass = enrollments.Any(x => x.ClassRoomId == classRoomId);

            if (!isMyClass)
            {
                return false;
            }

            if (classRoom.EndDate.Date > DateTime.Today)
            {
                return false;
            }

            if (classRoom.TeacherId <= 0)
            {
                return false;
            }

            if (rating < 1 || rating > 5)
            {
                return false;
            }

            var existingReview = await _teacherReviewRepository.GetByStudentAndClassRoomAsync(student.Id, classRoomId);

            if (existingReview == null)
            {
                var review = new TeacherReview
                {
                    TeacherId = classRoom.TeacherId,
                    StudentId = student.Id,
                    ClassRoomId = classRoomId,
                    Rating = rating,
                    Comment = comment,
                    CreatedAt = DateTime.Now
                };

                await _teacherReviewRepository.AddAsync(review);
            }
            else
            {
                existingReview.Rating = rating;
                existingReview.Comment = comment;
                existingReview.CreatedAt = DateTime.Now;

                _teacherReviewRepository.Update(existingReview);
            }

            await _teacherReviewRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _teacherReviewRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return false;
            }

            _teacherReviewRepository.Delete(existing);
            await _teacherReviewRepository.SaveChangesAsync();
            return true;
        }
    }
}