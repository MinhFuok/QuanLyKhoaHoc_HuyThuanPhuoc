using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Domain.Entities;

namespace QLKH.Application.Interfaces.Services
{
    public interface IClassMaterialService
    {
        Task<IEnumerable<ClassMaterial>> GetAllAsync();
        Task<ClassMaterial?> GetByIdAsync(int id);
        Task<IEnumerable<ClassMaterial>> GetByClassRoomIdAsync(int classRoomId);
        Task<IEnumerable<ClassMaterial>> GetMyTeachingMaterialsAsync(string applicationUserId);
        Task<IEnumerable<ClassMaterial>> GetMyLearningMaterialsAsync(string applicationUserId);
        Task AddAsync(ClassMaterial classMaterial);
        Task UpdateAsync(ClassMaterial classMaterial);
        Task<bool> DeleteAsync(int id);
    }
}