using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKH.Application.ViewModels
{
    public class StudentGradeOverviewViewModel
    {
        public string StudentName { get; set; } = string.Empty;

        public List<StudentGradeItemViewModel> Items { get; set; } = new List<StudentGradeItemViewModel>();
    }
}
