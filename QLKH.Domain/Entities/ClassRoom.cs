using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKH.Domain.Entities
{
    public class ClassRoom
    {
        public int Id { get; set; }

        public string ClassCode { get; set; } = string.Empty;

        public string ClassName { get; set; } = string.Empty;

        public int CourseId { get; set; }

        public int TeacherId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int MaxStudents { get; set; }

        public Course? Course { get; set; }

        public Teacher? Teacher { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        public ICollection<ClassSchedule> ClassSchedules { get; set; } = new List<ClassSchedule>();
        public ICollection<ClassMaterial> ClassMaterials { get; set; } = new List<ClassMaterial>();
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
        public ICollection<TeacherReview> TeacherReviews { get; set; } = new List<TeacherReview>();
    }
}