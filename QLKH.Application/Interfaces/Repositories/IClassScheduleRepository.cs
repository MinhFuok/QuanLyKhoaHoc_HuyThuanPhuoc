using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Domain.Entities;

namespace QLKH.Application.Interfaces.Repositories
{
    public interface IClassScheduleRepository
    {
        Task<IEnumerable<ClassSchedule>> GetAllAsync();
        Task<ClassSchedule?> GetByIdAsync(int id);
        Task<IEnumerable<ClassSchedule>> GetByClassRoomIdAsync(int classRoomId);
        Task AddAsync(ClassSchedule classSchedule);
        void Update(ClassSchedule classSchedule);
        void Delete(ClassSchedule classSchedule);

        Task<bool> ExistsConflictAsync(
            DateTime studyDate,
            TimeSpan startTime,
            TimeSpan endTime,
            int? excludeId = null);

        Task SaveChangesAsync();
    }
}