using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKH.Application.ViewModels
{
    public class TeacherGradeOverviewViewModel
    {
        public int ClassRoomId { get; set; }

        public string ClassName { get; set; } = string.Empty;

        public string CourseName { get; set; } = string.Empty;

        public List<TeacherGradeAssignmentViewModel> Assignments { get; set; } = new List<TeacherGradeAssignmentViewModel>();

        public List<TeacherGradeStudentRowViewModel> Students { get; set; } = new List<TeacherGradeStudentRowViewModel>();
    }
}