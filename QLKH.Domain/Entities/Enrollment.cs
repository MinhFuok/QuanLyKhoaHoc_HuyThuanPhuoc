using System;
using QLKH.Domain.Enums;

namespace QLKH.Domain.Entities
{
    public class Enrollment
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public int ClassRoomId { get; set; }

        public DateTime EnrolledAt { get; set; } = DateTime.Now;

        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Pending;

        public Student? Student { get; set; }

        public ClassRoom? ClassRoom { get; set; }
    }
}