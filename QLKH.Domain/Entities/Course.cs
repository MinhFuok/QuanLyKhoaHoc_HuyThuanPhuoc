using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QLKH.Domain.Enums;

namespace QLKH.Domain.Entities
{
    public class Course
    {
        public int Id { get; set; }

        public string CourseCode { get; set; } = string.Empty;

        public string CourseName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int DurationInMonths { get; set; }

        public decimal Fee { get; set; }

        public CourseStatus Status { get; set; } = CourseStatus.Active;

        public ICollection<ClassRoom> ClassRooms { get; set; } = new List<ClassRoom>();
    }
}
