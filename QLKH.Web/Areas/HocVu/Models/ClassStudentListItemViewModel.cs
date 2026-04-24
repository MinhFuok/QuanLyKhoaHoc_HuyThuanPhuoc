using System;

namespace QLKH.Web.Areas.HocVu.Models
{
    public class ClassStudentListItemViewModel
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }

        public string StudentCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime EnrolledAt { get; set; }

        public string StatusText { get; set; } = string.Empty;
    }
}