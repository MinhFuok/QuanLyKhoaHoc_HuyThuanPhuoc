using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Domain.Entities;

namespace QLKH.Application.Interfaces.Services
{
    public interface IClassRoomService
    {
        Task<IEnumerable<ClassRoom>> GetAllAsync();
        Task<ClassRoom?> GetByIdAsync(int id);
        Task<ClassRoom?> GetByIdWithDetailsAsync(int id);
        Task<bool> CreateAsync(ClassRoom classRoom);
        Task<bool> UpdateAsync(ClassRoom classRoom);
        Task<bool> DeleteAsync(int id);
    }
}