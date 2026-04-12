using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKH.Domain.Entities
{
    public class ClassSchedule
    {
        public int Id { get; set; }

        public int ClassRoomId { get; set; }

        public string LessonTitle { get; set; } = string.Empty;

        public DateTime StudyDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public string? RoomName { get; set; }

        public string? Note { get; set; }

        public ClassRoom ClassRoom { get; set; } = null!;
    }
}