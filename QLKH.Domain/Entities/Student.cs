using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKH.Domain.Entities
{
    public class Student
    {
        public int Id { get; set; }

        public string StudentCode { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public string? Address { get; set; }

        // Liên kết tới AspNetUsers.Id
        public string? ApplicationUserId { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
        public ICollection<TeacherReview> TeacherReviews { get; set; } = new List<TeacherReview>();
        public ICollection<StudentCertificate>? StudentCertificates { get; set; }
    }
}