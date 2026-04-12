using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Domain.Entities;

namespace QLKH.Application.Interfaces.Services
{
    public interface ISubmissionService
    {
        Task<IEnumerable<Submission>> GetAllAsync();
        Task<Submission?> GetByIdAsync(int id);
        Task<IEnumerable<Submission>> GetByAssignmentIdAsync(int assignmentId);
        Task<IEnumerable<Submission>> GetMySubmissionsAsync(string applicationUserId);
        Task<bool> SubmitAsync(string applicationUserId, int assignmentId, string? submissionText, string? filePath);
        Task<bool> GradeSubmissionAsync(int submissionId, decimal? score, string? feedback);
        Task<Submission?> GetByAssignmentAndCurrentStudentAsync(string applicationUserId, int assignmentId);
        Task<QLKH.Application.ViewModels.StudentGradeOverviewViewModel?> GetStudentGradeOverviewAsync(string applicationUserId);
        Task<QLKH.Application.ViewModels.TeacherGradeOverviewViewModel?> GetTeacherGradeOverviewByClassAsync(int classRoomId);
        Task<QLKH.Application.ViewModels.StudentLearningProgressOverviewViewModel?> GetStudentLearningProgressOverviewAsync(string applicationUserId);
        Task<bool> DeleteAsync(int id);
    }
}