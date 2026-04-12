using Microsoft.EntityFrameworkCore;
using QLKH.Application.Interfaces.Repositories;
using QLKH.Domain.Entities;
using QLKH.Infrastructure.Data;

namespace QLKH.Infrastructure.Repositories
{
    public class ClassMaterialRepository : IClassMaterialRepository
    {
        private readonly AppDbContext _context;

        public ClassMaterialRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClassMaterial>> GetAllAsync()
        {
            return await _context.ClassMaterials
                .Include(x => x.ClassRoom)
                    .ThenInclude(x => x.Course)
                .Include(x => x.ClassRoom)
                    .ThenInclude(x => x.Teacher)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<ClassMaterial?> GetByIdAsync(int id)
        {
            return await _context.ClassMaterials
                .Include(x => x.ClassRoom)
                    .ThenInclude(x => x.Course)
                .Include(x => x.ClassRoom)
                    .ThenInclude(x => x.Teacher)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<ClassMaterial>> GetByClassRoomIdAsync(int classRoomId)
        {
            return await _context.ClassMaterials
                .Include(x => x.ClassRoom)
                    .ThenInclude(x => x.Course)
                .Include(x => x.ClassRoom)
                    .ThenInclude(x => x.Teacher)
                .Where(x => x.ClassRoomId == classRoomId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(ClassMaterial classMaterial)
        {
            await _context.ClassMaterials.AddAsync(classMaterial);
        }

        public void Update(ClassMaterial classMaterial)
        {
            _context.ClassMaterials.Update(classMaterial);
        }

        public void Delete(ClassMaterial classMaterial)
        {
            _context.ClassMaterials.Remove(classMaterial);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}