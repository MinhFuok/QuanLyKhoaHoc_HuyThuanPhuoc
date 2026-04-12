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
    public class SubmissionService : ISubmissionService
    {
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IClassRoomRepository _classRoomRepository;

        public SubmissionService(
            ISubmissionRepository submissionRepository,
            IStudentRepository studentRepository,
            IAssignmentRepository assignmentRepository,
            IEnrollmentRepository enrollmentRepository,
            IClassRoomRepository classRoomRepository)
        {
            _submissionRepository = submissionRepository;
            _studentRepository = studentRepository;
            _assignmentRepository = assignmentRepository;
            _enrollmentRepository = enrollmentRepository;
            _classRoomRepository = classRoomRepository;
        }

        public async Task<IEnumerable<Submission>> GetAllAsync()
        {
            return await _submissionRepository.GetAllAsync();
        }

        public async Task<Submission?> GetByIdAsync(int id)
        {
            return await _submissionRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Submission>> GetByAssignmentIdAsync(int assignmentId)
        {
            return await _submissionRepository.GetByAssignmentIdAsync(assignmentId);
        }

        public async Task<IEnumerable<Submission>> GetMySubmissionsAsync(string applicationUserId)
        {
            var student = await _studentRepository.GetByApplicationUserIdAsync(applicationUserId);

            if (student == null)
            {
                return Enumerable.Empty<Submission>();
            }

            return await _submissionRepository.GetByStudentIdAsync(student.Id);
        }

        public async Task<Submission?> GetByAssignmentAndCurrentStudentAsync(string applicationUserId, int assignmentId)
        {
            var student = await _studentRepository.GetByApplicationUserIdAsync(applicationUserId);

            if (student == null)
            {
                return null;
            }

            return await _submissionRepository.GetByAssignmentAndStudentAsync(assignmentId, student.Id);
        }

        public async Task<StudentGradeOverviewViewModel?> GetStudentGradeOverviewAsync(string applicationUserId)
        {
            var student = await _studentRepository.GetByApplicationUserIdAsync(applicationUserId);
            if (student == null)
            {
                return null;
            }

            var enrollments = await _enrollmentRepository.GetByStudentIdAsync(student.Id);
            var classRoomIds = enrollments
                .Select(x => x.ClassRoomId)
                .Distinct()
                .ToList();

            var allAssignments = new List<Assignment>();

            foreach (var classRoomId in classRoomIds)
            {
                var assignments = await _assignmentRepository.GetByClassRoomIdAsync(classRoomId);
                allAssignments.AddRange(assignments);
            }

            var studentSubmissions = await _submissionRepository.GetByStudentIdAsync(student.Id);

            var items = allAssignments
                .GroupBy(x => x.Id)
                .Select(g => g.First())
                .OrderByDescending(x => x.CreatedAt)
                .Select(assignment =>
                {
                    var submission = studentSubmissions
                        .FirstOrDefault(x => x.AssignmentId == assignment.Id);

                    return new StudentGradeItemViewModel
                    {
                        AssignmentId = assignment.Id,
                        AssignmentTitle = assignment.Title,
                        ClassName = assignment.ClassRoom?.ClassName ?? string.Empty,
                        DueDate = assignment.DueDate,
                        IsSubmitted = submission != null,
                        SubmittedAt = submission?.SubmittedAt,
                        Score = submission?.Score,
                        Feedback = submission?.Feedback
                    };
                })
                .ToList();

            return new StudentGradeOverviewViewModel
            {
                StudentName = student.FullName,
                Items = items
            };
        }

        public async Task<TeacherGradeOverviewViewModel?> GetTeacherGradeOverviewByClassAsync(int classRoomId)
        {
            var classRoom = await _classRoomRepository.GetByIdAsync(classRoomId);
            if (classRoom == null)
            {
                return null;
            }

            var enrollments = await _enrollmentRepository.GetAllAsync();
            var classEnrollments = enrollments
                .Where(x => x.ClassRoomId == classRoomId)
                .ToList();

            var students = classEnrollments
                .Where(x => x.Student != null)
                .Select(x => x.Student!)
                .GroupBy(x => x.Id)
                .Select(g => g.First())
                .OrderBy(x => x.FullName)
                .ToList();

            var assignments = await _assignmentRepository.GetByClassRoomIdAsync(classRoomId);
            var assignmentList = assignments
                .OrderBy(x => x.CreatedAt)
                .ToList();

            var allSubmissions = new List<Submission>();
            foreach (var assignment in assignmentList)
            {
                var submissions = await _submissionRepository.GetByAssignmentIdAsync(assignment.Id);
                allSubmissions.AddRange(submissions);
            }

            var studentRows = students
                .Select(student => new TeacherGradeStudentRowViewModel
                {
                    StudentId = student.Id,
                    StudentCode = student.StudentCode,
                    StudentName = student.FullName,
                    ScoresByAssignmentId = assignmentList.ToDictionary(
                        assignment => assignment.Id,
                        assignment =>
                        {
                            var submission = allSubmissions.FirstOrDefault(x =>
                                x.AssignmentId == assignment.Id && x.StudentId == student.Id);

                            return submission?.Score;
                        })
                })
                .ToList();

            return new TeacherGradeOverviewViewModel
            {
                ClassRoomId = classRoom.Id,
                ClassName = classRoom.ClassName,
                CourseName = classRoom.Course?.CourseName ?? string.Empty,
                Assignments = assignmentList
                    .Select(x => new TeacherGradeAssignmentViewModel
                    {
                        AssignmentId = x.Id,
                        AssignmentTitle = x.Title
                    })
                    .ToList(),
                Students = studentRows
            };
        }

        public async Task<bool> SubmitAsync(string applicationUserId, int assignmentId, string? submissionText, string? filePath)
        {
            var student = await _studentRepository.GetByApplicationUserIdAsync(applicationUserId);
            if (student == null)
            {
                return false;
            }

            var assignment = await _assignmentRepository.GetByIdAsync(assignmentId);
            if (assignment == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(submissionText) && string.IsNullOrWhiteSpace(filePath))
            {
                return false;
            }

            var existingSubmission = await _submissionRepository.GetByAssignmentAndStudentAsync(assignmentId, student.Id);

            if (existingSubmission == null)
            {
                var submission = new Submission
                {
                    AssignmentId = assignmentId,
                    StudentId = student.Id,
                    SubmissionText = submissionText,
                    FilePath = filePath,
                    SubmittedAt = DateTime.Now
                };

                await _submissionRepository.AddAsync(submission);
            }
            else
            {
                existingSubmission.SubmissionText = submissionText;
                existingSubmission.FilePath = filePath;
                existingSubmission.SubmittedAt = DateTime.Now;

                _submissionRepository.Update(existingSubmission);
            }

            await _submissionRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> GradeSubmissionAsync(int submissionId, decimal? score, string? feedback)
        {
            var submission = await _submissionRepository.GetByIdAsync(submissionId);
            if (submission == null)
            {
                return false;
            }

            submission.Score = score;
            submission.Feedback = feedback;

            _submissionRepository.Update(submission);
            await _submissionRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _submissionRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return false;
            }

            _submissionRepository.Delete(existing);
            await _submissionRepository.SaveChangesAsync();
            return true;
        }
        public async Task<StudentLearningProgressOverviewViewModel?> GetStudentLearningProgressOverviewAsync(string applicationUserId)
        {
            var student = await _studentRepository.GetByApplicationUserIdAsync(applicationUserId);
            if (student == null)
            {
                return null;
            }

            var enrollments = await _enrollmentRepository.GetByStudentIdAsync(student.Id);
            var classRoomIds = enrollments
                .Select(x => x.ClassRoomId)
                .Distinct()
                .ToList();

            var allAssignments = new List<Assignment>();

            foreach (var classRoomId in classRoomIds)
            {
                var assignments = await _assignmentRepository.GetByClassRoomIdAsync(classRoomId);
                allAssignments.AddRange(assignments);
            }

            allAssignments = allAssignments
                .GroupBy(x => x.Id)
                .Select(g => g.First())
                .ToList();

            var submissions = await _submissionRepository.GetByStudentIdAsync(student.Id);

            var totalAssignments = allAssignments.Count;
            var submittedAssignments = submissions
                .Select(x => x.AssignmentId)
                .Distinct()
                .Count();

            var gradedAssignments = submissions.Count(x => x.Score.HasValue);

            var scoredSubmissions = submissions
                .Where(x => x.Score.HasValue)
                .Select(x => x.Score!.Value)
                .ToList();

            decimal? averageScore = scoredSubmissions.Any()
                ? scoredSubmissions.Average()
                : null;

            decimal overallCompletionPercent = 0;
            if (totalAssignments > 0)
            {
                overallCompletionPercent = Math.Round((decimal)submittedAssignments * 100 / totalAssignments, 2);
            }

            var classItems = classRoomIds
                .Select(classRoomId =>
                {
                    var classAssignments = allAssignments
                        .Where(x => x.ClassRoomId == classRoomId)
                        .ToList();

                    var submittedInClass = submissions
                        .Where(x => classAssignments.Any(a => a.Id == x.AssignmentId))
                        .Select(x => x.AssignmentId)
                        .Distinct()
                        .Count();

                    var gradedInClass = submissions
                        .Where(x => classAssignments.Any(a => a.Id == x.AssignmentId) && x.Score.HasValue)
                        .Count();

                    var classRoom = classAssignments.FirstOrDefault()?.ClassRoom;

                    decimal completionPercent = 0;
                    if (classAssignments.Count > 0)
                    {
                        completionPercent = Math.Round((decimal)submittedInClass * 100 / classAssignments.Count, 2);
                    }

                    return new StudentLearningProgressClassItemViewModel
                    {
                        ClassRoomId = classRoomId,
                        ClassName = classRoom?.ClassName ?? string.Empty,
                        TotalAssignments = classAssignments.Count,
                        SubmittedAssignments = submittedInClass,
                        GradedAssignments = gradedInClass,
                        CompletionPercent = completionPercent
                    };
                })
                .OrderBy(x => x.ClassName)
                .ToList();

            return new StudentLearningProgressOverviewViewModel
            {
                StudentName = student.FullName,
                TotalClasses = classRoomIds.Count,
                TotalAssignments = totalAssignments,
                SubmittedAssignments = submittedAssignments,
                PendingAssignments = totalAssignments - submittedAssignments,
                GradedAssignments = gradedAssignments,
                AverageScore = averageScore.HasValue ? Math.Round(averageScore.Value, 2) : null,
                OverallCompletionPercent = overallCompletionPercent,
                Classes = classItems
            };
        }
    }
}