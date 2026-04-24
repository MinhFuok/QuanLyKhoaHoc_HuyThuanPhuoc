using System;
using System.Collections.Generic;

namespace QLKH.Web.Areas.Admin.Models
{
    public class AdminDashboardViewModel
    {
        public string AdminFullName { get; set; } = string.Empty;
        public string AdminEmail { get; set; } = string.Empty;
        public string AdminRole { get; set; } = string.Empty;

        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int LockedUsers { get; set; }

        public int TotalStudents { get; set; }
        public int LinkedStudents { get; set; }

        public int TotalTeachers { get; set; }
        public int LinkedTeachers { get; set; }

        public int TotalCourses { get; set; }
        public int TotalClassRooms { get; set; }
        public int TotalDepartments { get; set; }

        public bool IsWebsiteEnabled { get; set; }
        public bool EnableEmail { get; set; }
        public int ActiveBannerCount { get; set; }
        public string SiteName { get; set; } = string.Empty;
        public string MaintenanceMessage { get; set; } = string.Empty;

        public int UnlinkedStudents => TotalStudents - LinkedStudents;
        public int UnlinkedTeachers => TotalTeachers - LinkedTeachers;

        public List<AdminDashboardActivityItemViewModel> RecentActivities { get; set; } = new();
    }

    public class AdminDashboardActivityItemViewModel
    {
        public DateTime CreatedAt { get; set; }
        public string ActorEmail { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public string TargetType { get; set; } = string.Empty;
        public string TargetDisplay { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
    }
}