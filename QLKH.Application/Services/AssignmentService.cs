using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Application.Interfaces.Repositories;
using QLKH.Application.Interfaces.Services;
using QLKH.Domain.Entities;

namespace QLKH.Application.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IClassRoomRepository _classRoomRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;

        public AssignmentService(
            IAssignmentRepository assignmentRepository,
            ITeacherRepository teacherRepository,
            IStudentRepository studentRepository,
            IClassRoomRepository classRoomRepository,
            IEnrollmentRepository enrollmentRepository)
        {
            _assignmentRepository = assignmentRepository;
            _teacherRepository = teacherRepository;
            _studentRepository = studentRepository;
            _classRoomRepository = classRoomRepository;
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<IEnumerable<Assignment>> GetAllAsync()
        {
            return await _assignmentRepository.GetAllAsync();
        }

        public async Task<Assignment?> GetByIdAsync(int id)
        {
            return await _assignmentRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Assignment>> GetByClassRoomIdAsync(int classRoomId)
        {
            return await _assignmentRepository.GetByClassRoomIdAsync(classRoomId);
        }

        public async Task<IEnumerable<Assignment>> GetMyTeachingAssignmentsAsync(string applicationUserId)
        {
            var teacher = await _teacherRepository.GetByApplicationUserIdAsync(applicationUserId);

            if (teacher == null)
            {
                return Enumerable.Empty<Assignment>();
            }

            var classRooms = await _classRoomRepository.GetByTeacherIdAsync(teacher.Id);

            var result = new List<Assignment>();

            foreach (var classRoom in classRooms)
            {
                var assignments = await _assignmentRepository.GetByClassRoomIdAsync(classRoom.Id);
                result.AddRange(assignments);
            }

            return result
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }

        public async Task<IEnumerable<Assignment>> GetMyLearningAssignmentsAsync(string applicationUserId)
        {
            var student = await _studentRepository.GetByApplicationUserIdAsync(applicationUserId);

            if (student == null)
            {
                return Enumerable.Empty<Assignment>();
            }

            var enrollments = await _enrollmentRepository.GetByStudentIdAsync(student.Id);

            var classRoomIds = enrollments
                .Select(x => x.ClassRoomId)
                .Distinct()
                .ToList();

            var result = new List<Assignment>();

            foreach (var classRoomId in classRoomIds)
            {
                var assignments = await _assignmentRepository.GetByClassRoomIdAsync(classRoomId);
                result.AddRange(assignments);
            }

            return result
                .GroupBy(x => x.Id)
                .Select(g => g.First())
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }
        public async Task AddAsync(Assignment assignment)
        {
            await _assignmentRepository.AddAsync(assignment);
            await _assignmentRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(Assignment assignment)
        {
            _assignmentRepository.Update(assignment);
            await _assignmentRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _assignmentRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return false;
            }

            _assignmentRepository.Delete(existing);
            await _assignmentRepository.SaveChangesAsync();
            return true;
        }
    }
}