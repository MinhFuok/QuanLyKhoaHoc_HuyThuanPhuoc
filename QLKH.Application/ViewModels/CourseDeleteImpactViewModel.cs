using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace QLKH.Application.ViewModels
{
    public class CourseDeleteImpactViewModel
    {
        public int CourseId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;

        public int ClassRoomCount { get; set; }
        public int EnrollmentCount { get; set; }
        public int ScheduleCount { get; set; }
        public int MaterialCount { get; set; }
        public int AssignmentCount { get; set; }
        public int SubmissionCount { get; set; }
        public int ReviewCount { get; set; }

        public bool HasDependencies =>
            ClassRoomCount > 0 ||
            EnrollmentCount > 0 ||
            ScheduleCount > 0 ||
            MaterialCount > 0 ||
            AssignmentCount > 0 ||
            SubmissionCount > 0 ||
            ReviewCount > 0;

        public bool ConfirmCascadeDelete { get; set; }
    }
}