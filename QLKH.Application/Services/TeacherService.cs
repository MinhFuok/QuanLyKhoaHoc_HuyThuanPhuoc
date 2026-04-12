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
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IClassRoomRepository _classRoomRepository;

        public TeacherService(
            ITeacherRepository teacherRepository,
            IClassRoomRepository classRoomRepository)
        {
            _teacherRepository = teacherRepository;
            _classRoomRepository = classRoomRepository;
        }

        public async Task<IEnumerable<Teacher>> GetAllAsync()
        {
            return await _teacherRepository.GetAllAsync();
        }

        public async Task<Teacher?> GetByIdAsync(int id)
        {
            return await _teacherRepository.GetByIdAsync(id);
        }

        public async Task<Teacher?> GetByApplicationUserIdAsync(string applicationUserId)
        {
            return await _teacherRepository.GetByApplicationUserIdAsync(applicationUserId);
        }

        public async Task<IEnumerable<ClassRoom>> GetMyTeachingClassesAsync(string applicationUserId)
        {
            var teacher = await _teacherRepository.GetByApplicationUserIdAsync(applicationUserId);

            if (teacher == null)
            {
                return Enumerable.Empty<ClassRoom>();
            }

            return await _classRoomRepository.GetByTeacherIdAsync(teacher.Id);
        }

        public async Task<bool> CreateAsync(Teacher teacher)
        {
            if (await _teacherRepository.ExistsByCodeAsync(teacher.TeacherCode))
            {
                return false;
            }

            await _teacherRepository.AddAsync(teacher);
            await _teacherRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(Teacher teacher)
        {
            var existingTeacher = await _teacherRepository.GetByIdAsync(teacher.Id);
            if (existingTeacher == null)
            {
                return false;
            }

            existingTeacher.TeacherCode = teacher.TeacherCode;
            existingTeacher.FullName = teacher.FullName;
            existingTeacher.Email = teacher.Email;
            existingTeacher.PhoneNumber = teacher.PhoneNumber;
            existingTeacher.Specialization = teacher.Specialization;

            _teacherRepository.Update(existingTeacher);
            await _teacherRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existingTeacher = await _teacherRepository.GetByIdAsync(id);
            if (existingTeacher == null)
            {
                return false;
            }

            _teacherRepository.Delete(existingTeacher);
            await _teacherRepository.SaveChangesAsync();
            return true;
        }
    }
}