using QLKH.Application.Interfaces.Messaging;
using QLKH.Application.Interfaces.Repositories;
using QLKH.Application.Interfaces.Services;
using QLKH.Application.Messages;
using QLKH.Domain.Entities;

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
            if (!await _studentRepository.ExistsAsync(enrollment.StudentId))
            {
                Console.WriteLine("Student does not exist");
                return false;
            }

            var classRoom = await _classRoomRepository.GetByIdAsync(enrollment.ClassRoomId);
            if (classRoom == null)
            {
                Console.WriteLine("ClassRoom does not exist");
                return false;
            }

            if (await _enrollmentRepository.IsStudentEnrolledInClassAsync(enrollment.StudentId, enrollment.ClassRoomId))
            {
                Console.WriteLine("Student already enrolled in this class");
                return false;
            }

            var currentStudentCount = await _enrollmentRepository.CountByClassRoomIdAsync(enrollment.ClassRoomId);
            if (currentStudentCount >= classRoom.MaxStudents)
            {
                Console.WriteLine("Class is full");
                return false;
            }

            await _enrollmentRepository.AddAsync(enrollment);
            await _enrollmentRepository.SaveChangesAsync();

            Console.WriteLine("Saved enrollment to database");

            var message = new EnrollmentCreatedMessage
            {
                EnrollmentId = enrollment.Id,
                StudentId = enrollment.StudentId,
                ClassRoomId = enrollment.ClassRoomId,
                EnrolledAt = enrollment.EnrolledAt,
                Status = enrollment.Status.ToString()
            };

            Console.WriteLine("Before publish RabbitMQ");
            _messagePublisher.Publish("enrollment-created-queue", message);
            Console.WriteLine("After publish RabbitMQ");

            return true;
        }

        public async Task<bool> UpdateAsync(Enrollment enrollment)
        {
            var existingEnrollment = await _enrollmentRepository.GetByIdAsync(enrollment.Id);
            if (existingEnrollment == null)
            {
                return false;
            }

            existingEnrollment.Status = enrollment.Status;

            _enrollmentRepository.Update(existingEnrollment);
            await _enrollmentRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existingEnrollment = await _enrollmentRepository.GetByIdAsync(id);
            if (existingEnrollment == null)
            {
                return false;
            }

            _enrollmentRepository.Delete(existingEnrollment);
            await _enrollmentRepository.SaveChangesAsync();
            return true;
        }
    }
}