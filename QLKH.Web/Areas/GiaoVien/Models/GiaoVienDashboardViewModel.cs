using System;
using System.Collections.Generic;

namespace QLKH.Web.Areas.GiaoVien.Models
{
    public class GiaoVienDashboardViewModel
    {
        public string TeacherFullName { get; set; } = string.Empty;
        public string TeacherEmail { get; set; } = string.Empty;
        public string TeacherRole { get; set; } = "Giáo viên";
        public string TeacherCode { get; set; } = string.Empty;
        public string? Specialization { get; set; }

        public int TotalClasses { get; set; }
        public int ActiveClasses { get; set; }
        public int EndedClasses { get; set; }

        public int TotalStudents { get; set; }
        public int PendingEnrollments { get; set; }

        public int TotalSchedules { get; set; }
        public int TodaySchedules { get; set; }
        public int UpcomingSchedulesCount { get; set; }

        public int TotalMaterials { get; set; }
        public int TotalAssignments { get; set; }

        public int TotalSubmissions { get; set; }
        public int GradedSubmissions { get; set; }
        public int UngradedSubmissions { get; set; }

        public List<GiaoVienScheduleItemViewModel> UpcomingSchedules { get; set; } = new();
        public List<GiaoVienAssignmentItemViewModel> LatestAssignments { get; set; } = new();
        public List<string> RecentNotes { get; set; } = new();
    }

    public class GiaoVienScheduleItemViewModel
    {
        public string ClassName { get; set; } = string.Empty;
        public string LessonTitle { get; set; } = string.Empty;
        public DateTime StudyDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? RoomName { get; set; }
    }

    public class GiaoVienAssignmentItemViewModel
    {
        public int AssignmentId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}