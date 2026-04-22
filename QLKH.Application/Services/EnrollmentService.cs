using QLKH.Application.Interfaces.Messaging;
using QLKH.Application.Interfaces.Repositories;
using QLKH.Application.Interfaces.Services;
using QLKH.Application.Messages;
using QLKH.Domain.Entities;
using QLKH.Domain.Enums;

namespace QLKH.Application.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IClassRoomRepository _classRoomRepository;
        private readonly IMessagePublisher _messagePublisher;

        public EnrollmentService(
            IEnrollmentRepository enrollmentRepository,
            IStudentRepository studentRepository,
            IClassRoomRepository classRoomRepository,
            IMessagePublisher messagePublisher)
        {
            _enrollmentRepository = enrollmentRepository;
            _studentRepository = studentRepository;
            _classRoomRepository = classRoomRepository;
            _messagePublisher = messagePublisher;
        }

        public async Task<IEnumerable<Enrollment>> GetAllAsync()
        {
            return await _enrollmentRepository.GetAllAsync();
        }

        public async Task<Enrollment?> GetByIdAsync(int id)
        {
            return await _enrollmentRepository.GetByIdAsync(id);
        }

        public async Task<Enrollment?> GetByIdWithDetailsAsync(int id)
        {
            return await _enrollmentRepository.GetByIdWithDetailsAsync(id);
        }

        public async Task<bool> CreateAsync(Enrollment enrollment)
        {
            var student = await _studentRepository.GetByIdAsync(enrollment.StudentId);
            var classRoom = await _classRoomRepository.GetByIdAsync(enrollment.ClassRoomId);

            if (student == null || classRoom == null)
            {
                return false;
            }

            var existingEnrollments = await _enrollmentRepository.GetAllAsync();
            var duplicated = existingEnrollments.Any(e =>
                e.StudentId == enrollment.StudentId &&
                e.ClassRoomId == enrollment.ClassRoomId);

            if (duplicated)
            {
                return false;
            }

            enrollment.EnrolledAt = DateTime.Now;
            enrollment.Status = EnrollmentStatus.Pending;

            await _enrollmentRepository.AddAsync(enrollment);
            return true;
        }

        public async Task<bool> UpdateAsync(Enrollment enrollment)
        {
            var existing = await _enrollmentRepository.GetByIdAsync(enrollment.Id);
            if (existing == null)
            {
                return false;
            }

            existing.StudentId = enrollment.StudentId;
            existing.ClassRoomId = enrollment.ClassRoomId;
            existing.Status = enrollment.Status;

            await _enrollmentRepository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(id);
            if (enrollment == null)
            {
                return false;
            }

            await _enrollmentRepository.DeleteAsync(enrollment);
            return true;
        }

        public async Task<bool> ChangeStatusAsync(int enrollmentId, EnrollmentStatus newStatus)
        {
            var enrollment = await _enrollmentRepository.GetByIdWithDetailsAsync(enrollmentId);
            if (enrollment == null)
            {
                return false;
            }

            if (enrollment.Status == newStatus)
            {
                return true;
            }

            if (newStatus == EnrollmentStatus.Confirmed)
            {
                var allEnrollments = await _enrollmentRepository.GetAllAsync();

                var confirmedCount = allEnrollments.Count(e =>
                    e.ClassRoomId == enrollment.ClassRoomId &&
                    e.Status == EnrollmentStatus.Confirmed &&
                    e.Id != enrollment.Id);

                if (enrollment.ClassRoom == null)
                {
                    return false;
                }

                if (confirmedCount >= enrollment.ClassRoom.MaxStudents)
                {
                    return false;
                }
            }

            enrollment.Status = newStatus;
            await _enrollmentRepository.UpdateAsync(enrollment);

            if (newStatus == EnrollmentStatus.Confirmed)
            {
                var message = new EnrollmentCreatedMessage
                {
                    EnrollmentId = enrollment.Id,
                    StudentId = enrollment.StudentId,
                    ClassRoomId = enrollment.ClassRoomId,
                    EnrolledAt = enrollment.EnrolledAt,
                    Status = enrollment.Status.ToString()
                };

                _messagePublisher.Publish("enrollment.created", message);
            }

            return true;
        }
    }
}