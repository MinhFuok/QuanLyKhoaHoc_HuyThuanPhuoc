using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Domain.Entities;

namespace QLKH.Application.Interfaces.Services
{
    public interface ITeacherReviewService
    {
        Task<IEnumerable<TeacherReview>> GetAllAsync();
        Task<TeacherReview?> GetByIdAsync(int id);
        Task<IEnumerable<TeacherReview>> GetMyTeacherReviewsAsync(string applicationUserId);
        Task<TeacherReview?> GetMyReviewForClassAsync(string applicationUserId, int classRoomId);
        Task<bool> SubmitReviewAsync(string applicationUserId, int classRoomId, int rating, string? comment);
        Task<bool> DeleteAsync(int id);
    }
}   
