using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKH.Application.ViewModels
{
    public class TeacherGradeStudentRowViewModel
    {
        public int StudentId { get; set; }

        public string StudentCode { get; set; } = string.Empty;

        public string StudentName { get; set; } = string.Empty;

        public Dictionary<int, decimal?> ScoresByAssignmentId { get; set; } = new Dictionary<int, decimal?>();
    }
}