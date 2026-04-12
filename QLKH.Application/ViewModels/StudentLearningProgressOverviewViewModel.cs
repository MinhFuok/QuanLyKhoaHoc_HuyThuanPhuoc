using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKH.Application.ViewModels
{
    public class StudentLearningProgressOverviewViewModel
    {
        public string StudentName { get; set; } = string.Empty;

        public int TotalClasses { get; set; }

        public int TotalAssignments { get; set; }

        public int SubmittedAssignments { get; set; }

        public int PendingAssignments { get; set; }

        public int GradedAssignments { get; set; }

        public decimal? AverageScore { get; set; }
        public decimal OverallCompletionPercent { get; set; }
        public List<StudentLearningProgressClassItemViewModel> Classes { get; set; } = new List<StudentLearningProgressClassItemViewModel>();
    }
}