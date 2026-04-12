using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Domain.Entities;

namespace QLKH.Application.Interfaces.Repositories
{
    public interface ITeacherReviewRepository
    {
        Task<IEnumerable<TeacherReview>> GetAllAsync();
        Task<TeacherReview?> GetByIdAsync(int id);
        Task<TeacherReview?> GetByStudentAndClassRoomAsync(int studentId, int classRoomId);
        Task<IEnumerable<TeacherReview>> GetByTeacherIdAsync(int teacherId);
        Task<IEnumerable<TeacherReview>> GetByClassRoomIdAsync(int classRoomId);
        Task AddAsync(TeacherReview teacherReview);
        void Update(TeacherReview teacherReview);
        void Delete(TeacherReview teacherReview);
        Task SaveChangesAsync();
    }
}