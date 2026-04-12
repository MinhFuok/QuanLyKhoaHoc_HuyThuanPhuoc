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
    public class ClassScheduleService : IClassScheduleService
    {
        private readonly IClassScheduleRepository _classScheduleRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IClassRoomRepository _classRoomRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;

        public ClassScheduleService(
            IClassScheduleRepository classScheduleRepository,
            ITeacherRepository teacherRepository,
            IStudentRepository studentRepository,
            IClassRoomRepository classRoomRepository,
            IEnrollmentRepository enrollmentRepository)
        {
            _classScheduleRepository = classScheduleRepository;
            _teacherRepository = teacherRepository;
            _studentRepository = studentRepository;
            _classRoomRepository = classRoomRepository;
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<IEnumerable<ClassSchedule>> GetAllAsync()
        {
            return await _classScheduleRepository.GetAllAsync();
        }

        public async Task<ClassSchedule?> GetByIdAsync(int id)
        {
            return await _classScheduleRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ClassSchedule>> GetByClassRoomIdAsync(int classRoomId)
        {
            return await _classScheduleRepository.GetByClassRoomIdAsync(classRoomId);
        }

        public async Task<IEnumerable<ClassSchedule>> GetMyTeachingScheduleAsync(string applicationUserId)
        {
            var teacher = await _teacherRepository.GetByApplicationUserIdAsync(applicationUserId);

            if (teacher == null)
            {
                return Enumerable.Empty<ClassSchedule>();
            }

            var classRooms = await _classRoomRepository.GetByTeacherIdAsync(teacher.Id);

            var result = new List<ClassSchedule>();

            foreach (var classRoom in classRooms)
            {
                var schedules = await _classScheduleRepository.GetByClassRoomIdAsync(classRoom.Id);
                result.AddRange(schedules);
            }

            return result
                .OrderBy(x => x.StudyDate)
                .ThenBy(x => x.StartTime)
                .ToList();
        }

        public async Task<IEnumerable<ClassSchedule>> GetMyLearningScheduleAsync(string applicationUserId)
        {
            var student = await _studentRepository.GetByApplicationUserIdAsync(applicationUserId);

            if (student == null)
            {
                return Enumerable.Empty<ClassSchedule>();
            }

            var enrollments = await _enrollmentRepository.GetByStudentIdAsync(student.Id);

            var classRoomIds = enrollments
                .Select(x => x.ClassRoomId)
                .Distinct()
                .ToList();

            var result = new List<ClassSchedule>();

            foreach (var classRoomId in classRoomIds)
            {
                var schedules = await _classScheduleRepository.GetByClassRoomIdAsync(classRoomId);
                result.AddRange(schedules);
            }

            return result
                .OrderBy(x => x.StudyDate)
                .ThenBy(x => x.StartTime)
                .ToList();
        }

        public async Task AddAsync(ClassSchedule classSchedule)
        {
            await _classScheduleRepository.AddAsync(classSchedule);
            await _classScheduleRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(ClassSchedule classSchedule)
        {
            _classScheduleRepository.Update(classSchedule);
            await _classScheduleRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _classScheduleRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return false;
            }

            _classScheduleRepository.Delete(existing);
            await _classScheduleRepository.SaveChangesAsync();
            return true;
        }
    }
}