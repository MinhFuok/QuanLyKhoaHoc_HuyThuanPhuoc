using System.Collections.Generic;

namespace QLKH.Web.Areas.HocVu.Models
{
    public class ClassStudentListViewModel
    {
        public int ClassRoomId { get; set; }
        public string ClassCode { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;

        public List<ClassStudentListItemViewModel> Students { get; set; } = new();
    }
}