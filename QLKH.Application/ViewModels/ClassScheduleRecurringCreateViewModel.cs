using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace QLKH.Application.ViewModels
{
    public class ClassScheduleRecurringCreateViewModel
    {
        [Display(Name = "Lớp học")]
        public int ClassRoomId { get; set; }

        [Display(Name = "Các ngày trong tuần")]
        public List<int> SelectedDays { get; set; } = new();

        [Required(ErrorMessage = "Vui lòng chọn buổi học.")]
        [Display(Name = "Buổi học")]
        public string Session { get; set; } = "Morning"; // Morning | Afternoon

        [Display(Name = "TeamCode")]
        [StringLength(100)]
        public string? TeamCode { get; set; }

        [Display(Name = "Ghi chú")]
        [StringLength(500)]
        public string? Note { get; set; }
    }
}