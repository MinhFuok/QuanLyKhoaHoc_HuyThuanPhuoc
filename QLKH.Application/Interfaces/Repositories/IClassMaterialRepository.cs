using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Domain.Entities;

namespace QLKH.Application.Interfaces.Repositories
{
    public interface IClassMaterialRepository
    {
        Task<IEnumerable<ClassMaterial>> GetAllAsync();
        Task<ClassMaterial?> GetByIdAsync(int id);
        Task<IEnumerable<ClassMaterial>> GetByClassRoomIdAsync(int classRoomId);
        Task AddAsync(ClassMaterial classMaterial);
        void Update(ClassMaterial classMaterial);
        void Delete(ClassMaterial classMaterial);
        Task SaveChangesAsync();
    }
}