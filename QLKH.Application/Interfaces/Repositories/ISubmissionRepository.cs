using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Domain.Entities;

namespace QLKH.Application.Interfaces.Repositories
{
    public interface ISubmissionRepository
    {
        Task<IEnumerable<Submission>> GetAllAsync();
        Task<Submission?> GetByIdAsync(int id);
        Task<Submission?> GetByAssignmentAndStudentAsync(int assignmentId, int studentId);
        Task<IEnumerable<Submission>> GetByAssignmentIdAsync(int assignmentId);
        Task<IEnumerable<Submission>> GetByStudentIdAsync(int studentId);
        Task AddAsync(Submission submission);
        void Update(Submission submission);
        void Delete(Submission submission);
        Task SaveChangesAsync();
    }
}