using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKH.Application.ViewModels
{
    public class StudentLearningProgressClassItemViewModel
    {
        public int ClassRoomId { get; set; }

        public string ClassName { get; set; } = string.Empty;

        public int TotalAssignments { get; set; }

        public int SubmittedAssignments { get; set; }

        public int GradedAssignments { get; set; }

        public decimal CompletionPercent { get; set; }
    }
}
