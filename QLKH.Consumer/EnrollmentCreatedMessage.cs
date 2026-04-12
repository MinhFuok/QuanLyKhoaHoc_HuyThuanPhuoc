using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKH.Consumer
{
    public class EnrollmentCreatedMessage
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int ClassRoomId { get; set; }
        public DateTime EnrolledAt { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}