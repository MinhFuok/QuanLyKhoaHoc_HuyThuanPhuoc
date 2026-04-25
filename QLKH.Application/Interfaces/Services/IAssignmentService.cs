using QLKH.Application.ViewModels;
using QLKH.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKH.Application.Interfaces.Services
{
    public interface IAssignmentService
    {
        Task<IEnumerable<Assignment>> GetAllAsync();
        Task<Assignment?> GetByIdAsync(int id);
        Task<IEnumerable<Assignment>> GetByClassRoomIdAsync(int classRoomId);
        Task<IEnumerable<Assignment>> GetMyTeachingAssignmentsAsync(string applicationUserId);
        Task<IEnumerable<Assignment>> GetMyLearningAssignmentsAsync(string applicationUserId);
        Task AddAsync(Assignment assignment);
        Task UpdateAsync(Assignment assignment);
        Task<bool> DeleteAsync(int id);
    }
}