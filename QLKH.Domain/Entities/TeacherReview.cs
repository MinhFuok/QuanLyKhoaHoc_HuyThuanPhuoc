using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKH.Domain.Entities
{
    public class TeacherReview
    {
        public int Id { get; set; }

        public int TeacherId { get; set; }

        public int StudentId { get; set; }

        public int ClassRoomId { get; set; }

        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; }

        public Teacher Teacher { get; set; } = null!;

        public Student Student { get; set; } = null!;

        public ClassRoom ClassRoom { get; set; } = null!;
    }
}