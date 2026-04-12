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
    public class SubmissionRepository : ISubmissionRepository
    {
        private readonly AppDbContext _context;

        public SubmissionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Submission>> GetAllAsync()
        {
            return await _context.Submissions
                .Include(x => x.Assignment)
                    .ThenInclude(x => x.ClassRoom)
                        .ThenInclude(x => x.Course)
                .Include(x => x.Assignment)
                    .ThenInclude(x => x.ClassRoom)
                        .ThenInclude(x => x.Teacher)
                .Include(x => x.Student)
                .OrderByDescending(x => x.SubmittedAt)
                .ToListAsync();
        }

        public async Task<Submission?> GetByIdAsync(int id)
        {
            return await _context.Submissions
                .Include(x => x.Assignment)
                    .ThenInclude(x => x.ClassRoom)
                        .ThenInclude(x => x.Course)
                .Include(x => x.Assignment)
                    .ThenInclude(x => x.ClassRoom)
                        .ThenInclude(x => x.Teacher)
                .Include(x => x.Student)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Submission?> GetByAssignmentAndStudentAsync(int assignmentId, int studentId)
        {
            return await _context.Submissions
                .Include(x => x.Assignment)
                    .ThenInclude(x => x.ClassRoom)
                        .ThenInclude(x => x.Course)
                .Include(x => x.Assignment)
                    .ThenInclude(x => x.ClassRoom)
                        .ThenInclude(x => x.Teacher)
                .Include(x => x.Student)
                .FirstOrDefaultAsync(x => x.AssignmentId == assignmentId && x.StudentId == studentId);
        }

        public async Task<IEnumerable<Submission>> GetByAssignmentIdAsync(int assignmentId)
        {
            return await _context.Submissions
                .Include(x => x.Assignment)
                    .ThenInclude(x => x.ClassRoom)
                        .ThenInclude(x => x.Course)
                .Include(x => x.Assignment)
                    .ThenInclude(x => x.ClassRoom)
                        .ThenInclude(x => x.Teacher)
                .Include(x => x.Student)
                .Where(x => x.AssignmentId == assignmentId)
                .OrderByDescending(x => x.SubmittedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Submission>> GetByStudentIdAsync(int studentId)
        {
            return await _context.Submissions
                .Include(x => x.Assignment)
                    .ThenInclude(x => x.ClassRoom)
                        .ThenInclude(x => x.Course)
                .Include(x => x.Assignment)
                    .ThenInclude(x => x.ClassRoom)
                        .ThenInclude(x => x.Teacher)
                .Include(x => x.Student)
                .Where(x => x.StudentId == studentId)
                .OrderByDescending(x => x.SubmittedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Submission submission)
        {
            await _context.Submissions.AddAsync(submission);
        }

        public void Update(Submission submission)
        {
            _context.Submissions.Update(submission);
        }

        public void Delete(Submission submission)
        {
            _context.Submissions.Remove(submission);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}