using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QLKH.Application.Interfaces.Repositories;
using QLKH.Domain.Entities;
using QLKH.Infrastructure.Data;

namespace QLKH.Infrastructure.Repositories
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly AppDbContext _context;

        public AssignmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Assignment>> GetAllAsync()
        {
            return await _context.Assignments
                .Include(x => x.ClassRoom)
                    .ThenInclude(x => x.Course)
                .Include(x => x.ClassRoom)
                    .ThenInclude(x => x.Teacher)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<Assignment?> GetByIdAsync(int id)
        {
            return await _context.Assignments
                .Include(x => x.ClassRoom)
                    .ThenInclude(x => x.Course)
                .Include(x => x.ClassRoom)
                    .ThenInclude(x => x.Teacher)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Assignment>> GetByClassRoomIdAsync(int classRoomId)
        {
            return await _context.Assignments
                .Include(x => x.ClassRoom)
                    .ThenInclude(x => x.Course)
                .Include(x => x.ClassRoom)
                    .ThenInclude(x => x.Teacher)
                .Where(x => x.ClassRoomId == classRoomId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Assignment assignment)
        {
            await _context.Assignments.AddAsync(assignment);
        }

        public void Update(Assignment assignment)
        {
            _context.Assignments.Update(assignment);
        }

        public void Delete(Assignment assignment)
        {
            _context.Assignments.Remove(assignment);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}