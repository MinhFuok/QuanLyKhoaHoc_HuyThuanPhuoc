using QLKH.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Application.Interfaces.Repositories;
using QLKH.Domain.Entities;

namespace QLKH.Application.Services
{
    public class ClassMaterialService : IClassMaterialService
    {
        private readonly IClassMaterialRepository _classMaterialRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IClassRoomRepository _classRoomRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;

        public ClassMaterialService(
            IClassMaterialRepository classMaterialRepository,
            ITeacherRepository teacherRepository,
            IStudentRepository studentRepository,
            IClassRoomRepository classRoomRepository,
            IEnrollmentRepository enrollmentRepository)
        {
            _classMaterialRepository = classMaterialRepository;
            _teacherRepository = teacherRepository;
            _studentRepository = studentRepository;
            _classRoomRepository = classRoomRepository;
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<IEnumerable<ClassMaterial>> GetAllAsync()
        {
            return await _classMaterialRepository.GetAllAsync();
        }

        public async Task<ClassMaterial?> GetByIdAsync(int id)
        {
            return await _classMaterialRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ClassMaterial>> GetByClassRoomIdAsync(int classRoomId)
        {
            return await _classMaterialRepository.GetByClassRoomIdAsync(classRoomId);
        }

        public async Task<IEnumerable<ClassMaterial>> GetMyTeachingMaterialsAsync(string applicationUserId)
        {
            var teacher = await _teacherRepository.GetByApplicationUserIdAsync(applicationUserId);

            if (teacher == null)
            {
                return Enumerable.Empty<ClassMaterial>();
            }

            var classRooms = await _classRoomRepository.GetByTeacherIdAsync(teacher.Id);

            var result = new List<ClassMaterial>();

            foreach (var classRoom in classRooms)
            {
                var materials = await _classMaterialRepository.GetByClassRoomIdAsync(classRoom.Id);
                result.AddRange(materials);
            }

            return result
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }

        public async Task<IEnumerable<ClassMaterial>> GetMyLearningMaterialsAsync(string applicationUserId)
        {
            var student = await _studentRepository.GetByApplicationUserIdAsync(applicationUserId);

            if (student == null)
            {
                return Enumerable.Empty<ClassMaterial>();
            }

            var enrollments = await _enrollmentRepository.GetByStudentIdAsync(student.Id);

            var classRoomIds = enrollments
                .Select(x => x.ClassRoomId)
                .Distinct()
                .ToList();

            var result = new List<ClassMaterial>();

            foreach (var classRoomId in classRoomIds)
            {
                var materials = await _classMaterialRepository.GetByClassRoomIdAsync(classRoomId);
                result.AddRange(materials);
            }

            return result
                .GroupBy(x => x.Id)
                .Select(g => g.First())
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }

        public async Task AddAsync(ClassMaterial classMaterial)
        {
            await _classMaterialRepository.AddAsync(classMaterial);
            await _classMaterialRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(ClassMaterial classMaterial)
        {
            _classMaterialRepository.Update(classMaterial);
            await _classMaterialRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _classMaterialRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return false;
            }

            _classMaterialRepository.Delete(existing);
            await _classMaterialRepository.SaveChangesAsync();
            return true;
        }
    }
}