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
    public class ClassRoomService : IClassRoomService
    {
        private readonly IClassRoomRepository _classRoomRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ITeacherRepository _teacherRepository;

        public ClassRoomService(
            IClassRoomRepository classRoomRepository,
            ICourseRepository courseRepository,
            ITeacherRepository teacherRepository)
        {
            _classRoomRepository = classRoomRepository;
            _courseRepository = courseRepository;
            _teacherRepository = teacherRepository;
        }

        public async Task<IEnumerable<ClassRoom>> GetAllAsync()
        {
            return await _classRoomRepository.GetAllAsync();
        }

        public async Task<ClassRoom?> GetByIdAsync(int id)
        {
            return await _classRoomRepository.GetByIdAsync(id);
        }

        public async Task<ClassRoom?> GetByIdWithDetailsAsync(int id)
        {
            return await _classRoomRepository.GetByIdWithDetailsAsync(id);
        }

        public async Task<bool> CreateAsync(ClassRoom classRoom)
        {
            if (await _classRoomRepository.ExistsByCodeAsync(classRoom.ClassCode))
            {
                return false;
            }

            if (!await _courseRepository.ExistsAsync(classRoom.CourseId))
            {
                return false;
            }

            if (!await _teacherRepository.ExistsAsync(classRoom.TeacherId))
            {
                return false;
            }

            await _classRoomRepository.AddAsync(classRoom);
            await _classRoomRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(ClassRoom classRoom)
        {
            var existingClassRoom = await _classRoomRepository.GetByIdAsync(classRoom.Id);
            if (existingClassRoom == null)
            {
                return false;
            }

            if (!await _courseRepository.ExistsAsync(classRoom.CourseId))
            {
                return false;
            }

            if (!await _teacherRepository.ExistsAsync(classRoom.TeacherId))
            {
                return false;
            }

            existingClassRoom.ClassCode = classRoom.ClassCode;
            existingClassRoom.ClassName = classRoom.ClassName;
            existingClassRoom.CourseId = classRoom.CourseId;
            existingClassRoom.TeacherId = classRoom.TeacherId;
            existingClassRoom.StartDate = classRoom.StartDate;
            existingClassRoom.EndDate = classRoom.EndDate;
            existingClassRoom.MaxStudents = classRoom.MaxStudents;

            _classRoomRepository.Update(existingClassRoom);
            await _classRoomRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existingClassRoom = await _classRoomRepository.GetByIdAsync(id);
            if (existingClassRoom == null)
            {
                return false;
            }

            _classRoomRepository.Delete(existingClassRoom);
            await _classRoomRepository.SaveChangesAsync();
            return true;
        }
    }
}