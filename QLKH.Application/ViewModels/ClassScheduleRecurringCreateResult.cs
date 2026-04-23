using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKH.Application.ViewModels
{
    public class ClassScheduleRecurringCreateResult
    {
        public bool Success { get; set; }

        public int CreatedCount { get; set; }

        public List<DateTime> ConflictDates { get; set; } = new();

        public string? ErrorMessage { get; set; }
    }
}