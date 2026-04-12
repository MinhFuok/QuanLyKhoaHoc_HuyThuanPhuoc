using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKH.Application.ViewModels
{
    public class StudentGradeItemViewModel
    {
        public int AssignmentId { get; set; }

        public string AssignmentTitle { get; set; } = string.Empty;

        public string ClassName { get; set; } = string.Empty;

        public DateTime DueDate { get; set; }

        public bool IsSubmitted { get; set; }

        public DateTime? SubmittedAt { get; set; }

        public decimal? Score { get; set; }

        public string? Feedback { get; set; }
    }
}
