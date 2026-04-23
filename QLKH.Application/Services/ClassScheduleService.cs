using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Application.Interfaces.Repositories;
using QLKH.Application.Interfaces.Services;
using QLKH.Application.ViewModels;
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

        private static readonly TimeSpan MorningStart = new(7, 0, 0);
        private static readonly TimeSpan MorningEnd = new(11, 30, 0);
        private static readonly TimeSpan AfternoonStart = new(13, 0, 0);
        private static readonly TimeSpan AfternoonEnd = new(17, 0, 0);

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
            var existing = await _classScheduleRepository.GetByIdAsync(classSchedule.Id);
            if (existing == null)
            {
                throw new KeyNotFoundException("Không tìm thấy lịch học cần cập nhật.");
            }

            existing.ClassRoomId = classSchedule.ClassRoomId;
            existing.LessonTitle = classSchedule.LessonTitle;
            existing.StudyDate = classSchedule.StudyDate;
            existing.StartTime = classSchedule.StartTime;
            existing.EndTime = classSchedule.EndTime;
            existing.RoomName = classSchedule.RoomName;
            existing.Note = classSchedule.Note;

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

        public async Task<ClassScheduleRecurringCreateResult> CreateRecurringAsync(
            int classRoomId,
            List<int> selectedDays,
            string session,
            string? teamCode,
            string? note)
        {
            var result = new ClassScheduleRecurringCreateResult();

            var classRoom = await _classRoomRepository.GetByIdAsync(classRoomId);
            if (classRoom == null)
            {
                result.Success = false;
                result.ErrorMessage = "Lớp học không tồn tại.";
                return result;
            }

            if (selectedDays == null || !selectedDays.Any())
            {
                result.Success = false;
                result.ErrorMessage = "Vui lòng chọn ít nhất 1 ngày trong tuần.";
                return result;
            }

            var (startTime, endTime, sessionText) = GetSessionTime(session);

            var conflictDates = new List<DateTime>();
            var datesToCreate = new List<DateTime>();

            for (var date = classRoom.StartDate.Date; date <= classRoom.EndDate.Date; date = date.AddDays(1))
            {
                var dayNumber = (int)date.DayOfWeek; // Chủ nhật = 0, Thứ hai = 1, ...
                if (!selectedDays.Contains(dayNumber))
                {
                    continue;
                }

                var hasConflict = await _classScheduleRepository.ExistsConflictAsync(date, startTime, endTime);
                if (hasConflict)
                {
                    conflictDates.Add(date);
                }
                else
                {
                    datesToCreate.Add(date);
                }
            }

            if (conflictDates.Any())
            {
                result.Success = false;
                result.ConflictDates = conflictDates
                    .OrderBy(x => x)
                    .ToList();

                result.ErrorMessage = "Có lịch bị trùng với lớp khác.";
                return result;
            }

            foreach (var date in datesToCreate)
            {
                var classSchedule = new ClassSchedule
                {
                    ClassRoomId = classRoomId,
                    StudyDate = date,
                    StartTime = startTime,
                    EndTime = endTime,
                    LessonTitle = $"{classRoom.ClassCode} - {date:dd/MM/yyyy} - Buổi {sessionText}",
                    RoomName = teamCode,
                    Note = note
                };

                await _classScheduleRepository.AddAsync(classSchedule);
            }

            await _classScheduleRepository.SaveChangesAsync();

            result.Success = true;
            result.CreatedCount = datesToCreate.Count;
            return result;
        }

        private static (TimeSpan StartTime, TimeSpan EndTime, string SessionText) GetSessionTime(string session)
        {
            return session switch
            {
                "Morning" => (MorningStart, MorningEnd, "Sáng"),
                "Afternoon" => (AfternoonStart, AfternoonEnd, "Chiều"),
                _ => throw new InvalidOperationException("Buổi học không hợp lệ.")
            };
        }
        public async Task<int> UpdateGroupedSessionsAsync(int anchorId, string session, string? teamCode, string? note)
        {
            var anchor = await _classScheduleRepository.GetByIdAsync(anchorId);
            if (anchor == null)
            {
                return 0;
            }

            var schedules = await _classScheduleRepository.GetByClassRoomIdAsync(anchor.ClassRoomId);

            var groupSchedules = schedules
                .Where(x =>
                    x.ClassRoomId == anchor.ClassRoomId &&
                    x.StartTime == anchor.StartTime &&
                    x.EndTime == anchor.EndTime &&
                    string.Equals(x.RoomName ?? "", anchor.RoomName ?? "", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(x.Note ?? "", anchor.Note ?? "", StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.StudyDate)
                .ToList();

            if (!groupSchedules.Any())
            {
                return 0;
            }

            var (newStartTime, newEndTime, sessionText) = GetSessionTime(session);

            // Kiểm tra conflict cho từng buổi nếu đổi sang buổi khác
            foreach (var item in groupSchedules)
            {
                var hasConflict = await _classScheduleRepository.ExistsConflictAsync(
                    item.StudyDate.Date,
                    newStartTime,
                    newEndTime,
                    item.Id);

                if (hasConflict)
                {
                    throw new InvalidOperationException(
                        $"Không thể cập nhật chuỗi lịch vì ngày {item.StudyDate:dd/MM/yyyy} bị trùng với lịch khác.");
                }
            }

            int index = 1;
            foreach (var item in groupSchedules)
            {
                item.StartTime = newStartTime;
                item.EndTime = newEndTime;
                item.RoomName = teamCode;
                item.Note = note;
                item.LessonTitle = $"{anchor.ClassRoom?.ClassCode} - {item.StudyDate:dd/MM/yyyy} - Buổi {sessionText}";
                index++;
            }

            await _classScheduleRepository.SaveChangesAsync();
            return groupSchedules.Count;
        }
        public async Task<int> DeleteGroupedSessionsAsync(int anchorId)
        {
            var anchor = await _classScheduleRepository.GetByIdAsync(anchorId);
            if (anchor == null)
            {
                return 0;
            }

            var schedules = await _classScheduleRepository.GetByClassRoomIdAsync(anchor.ClassRoomId);

            var groupSchedules = schedules
                .Where(x =>
                    x.ClassRoomId == anchor.ClassRoomId &&
                    x.StartTime == anchor.StartTime &&
                    x.EndTime == anchor.EndTime &&
                    string.Equals(x.RoomName ?? "", anchor.RoomName ?? "", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(x.Note ?? "", anchor.Note ?? "", StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var item in groupSchedules)
            {
                _classScheduleRepository.Delete(item);
            }

            await _classScheduleRepository.SaveChangesAsync();
            return groupSchedules.Count;
        }
    }
}