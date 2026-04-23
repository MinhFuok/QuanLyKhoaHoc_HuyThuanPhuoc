using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace QLKH.Application.ViewModels
{
    public class ClassSchedulePickSessionViewModel
    {
        public int AnchorId { get; set; }

        [Display(Name = "Chọn buổi")]
        public int SelectedScheduleId { get; set; }

        public bool EditAllSessions { get; set; }

        public bool DeleteAllSessions { get; set; }

        public string ClassCode { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string? CourseName { get; set; }
        public string SessionText { get; set; } = string.Empty;
        public string TimeRangeText { get; set; } = string.Empty;
        public string? TeamCode { get; set; }
        public string? Note { get; set; }
        public int TotalSessions { get; set; }

        public List<ClassScheduleSessionOptionViewModel> SessionOptions { get; set; } = new();
    }

    public class ClassScheduleSessionOptionViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}