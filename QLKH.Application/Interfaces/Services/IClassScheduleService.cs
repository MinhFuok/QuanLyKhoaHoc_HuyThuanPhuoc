using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Application.ViewModels;
using QLKH.Domain.Entities;

namespace QLKH.Application.Interfaces.Services
{
    public interface IClassScheduleService
    {
        Task<IEnumerable<ClassSchedule>> GetAllAsync();
        Task<ClassSchedule?> GetByIdAsync(int id);
        Task<IEnumerable<ClassSchedule>> GetByClassRoomIdAsync(int classRoomId);
        Task<IEnumerable<ClassSchedule>> GetMyTeachingScheduleAsync(string applicationUserId);
        Task<IEnumerable<ClassSchedule>> GetMyLearningScheduleAsync(string applicationUserId);

        Task AddAsync(ClassSchedule classSchedule);
        Task UpdateAsync(ClassSchedule classSchedule);
        Task<bool> DeleteAsync(int id);

        Task<ClassScheduleRecurringCreateResult> CreateRecurringAsync(
            int classRoomId,
            List<int> selectedDays,
            string session,
            string? teamCode,
            string? note);
        Task<int> UpdateGroupedSessionsAsync(int anchorId, string session, string? teamCode, string? note);
        Task<int> DeleteGroupedSessionsAsync(int anchorId);
    }
}