using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKH.Domain.Entities
{
    public class Teacher
    {
        public int Id { get; set; }

        public string TeacherCode { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string? Specialization { get; set; }

        // Liên kết tới AspNetUsers.Id
        public string? ApplicationUserId { get; set; }

        public ICollection<ClassRoom> ClassRooms { get; set; } = new List<ClassRoom>();
        public ICollection<TeacherReview> TeacherReviews { get; set; } = new List<TeacherReview>();
    }
}