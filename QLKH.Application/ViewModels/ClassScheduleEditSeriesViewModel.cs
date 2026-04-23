using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace QLKH.Application.ViewModels
{
    public class ClassScheduleEditSeriesViewModel
    {
        public int AnchorId { get; set; }

        public string ClassCode { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string? CourseName { get; set; }

        public int TotalSessions { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn buổi học.")]
        [Display(Name = "Buổi học")]
        public string Session { get; set; } = "Morning";

        [Display(Name = "TeamCode")]
        [StringLength(100)]
        public string? TeamCode { get; set; }

        [Display(Name = "Ghi chú")]
        [StringLength(500)]
        public string? Note { get; set; }
    }
}