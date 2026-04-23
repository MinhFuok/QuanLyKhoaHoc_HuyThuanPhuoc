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
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;

        public StudentService(
            IStudentRepository studentRepository,
            IEnrollmentRepository enrollmentRepository)
        {
            _studentRepository = studentRepository;
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _studentRepository.GetAllAsync();
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            return await _studentRepository.GetByIdAsync(id);
        }

        public async Task<Student?> GetByApplicationUserIdAsync(string applicationUserId)
        {
            return await _studentRepository.GetByApplicationUserIdAsync(applicationUserId);
        }

        public async Task<IEnumerable<ClassRoom>> GetMyClassesAsync(string applicationUserId)
        {
            var student = await _studentRepository.GetByApplicationUserIdAsync(applicationUserId);

            if (student == null)
            {
                return Enumerable.Empty<ClassRoom>();
            }

            var enrollments = await _enrollmentRepository.GetByStudentIdAsync(student.Id);

            return enrollments
            .Where(x => x.ClassRoom != null)
            .Select(x => x.ClassRoom!)
            .GroupBy(x => x.Id)
            .Select(g => g.First())
            .ToList();
        }
        public async Task<bool> ExistsByApplicationUserIdAsync(string applicationUserId, int? excludeStudentId = null)
        {
            return await _studentRepository.ExistsByApplicationUserIdAsync(applicationUserId, excludeStudentId);
        }
        public async Task<bool> CreateAsync(Student student)
        {
            if (await _studentRepository.ExistsByCodeAsync(student.StudentCode))
            {
                return false;
            }

            await _studentRepository.AddAsync(student);
            await _studentRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(Student student)
        {
            var existingStudent = await _studentRepository.GetByIdAsync(student.Id);
            if (existingStudent == null)
            {
                return false;
            }

            existingStudent.StudentCode = student.StudentCode;
            existingStudent.FullName = student.FullName;
            existingStudent.Email = student.Email;
            existingStudent.PhoneNumber = student.PhoneNumber;
            existingStudent.DateOfBirth = student.DateOfBirth;
            existingStudent.Address = student.Address;

            _studentRepository.Update(existingStudent);
            await _studentRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existingStudent = await _studentRepository.GetByIdAsync(id);
            if (existingStudent == null)
            {
                return false;
            }

            _studentRepository.Delete(existingStudent);
            await _studentRepository.SaveChangesAsync();
            return true;
        }
    }
}