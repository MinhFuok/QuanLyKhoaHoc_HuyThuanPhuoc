using System.Collections.Generic;

namespace QLKH.Web.Areas.HocVu.Models
{
    public class HocVuDashboardViewModel
    {
        public string HocVuFullName { get; set; } = string.Empty;
        public string HocVuEmail { get; set; } = string.Empty;
        public string HocVuRole { get; set; } = "Học vụ";

        public int TotalCourses { get; set; }
        public int TotalClassRooms { get; set; }
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalEnrollments { get; set; }
        public int PendingEnrollments { get; set; }
        public int ConfirmedEnrollments { get; set; }
        public int CancelledEnrollments { get; set; }
        public int TotalCertificates { get; set; }

        public List<string> RecentNotes { get; set; } = new();
    }
}