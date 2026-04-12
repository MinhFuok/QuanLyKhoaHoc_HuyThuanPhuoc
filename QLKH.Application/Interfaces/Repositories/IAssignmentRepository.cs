using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Domain.Entities;

namespace QLKH.Application.Interfaces.Repositories
{
    public interface IAssignmentRepository
    {
        Task<IEnumerable<Assignment>> GetAllAsync();
        Task<Assignment?> GetByIdAsync(int id);
        Task<IEnumerable<Assignment>> GetByClassRoomIdAsync(int classRoomId);
        Task AddAsync(Assignment assignment);
        void Update(Assignment assignment);
        void Delete(Assignment assignment);
        Task SaveChangesAsync();
    }
}
