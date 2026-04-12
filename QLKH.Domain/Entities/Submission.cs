using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKH.Domain.Entities
{
    public class Submission
    {
        public int Id { get; set; }

        public int AssignmentId { get; set; }

        public int StudentId { get; set; }

        public string? SubmissionText { get; set; }

        public string? FilePath { get; set; }

        public DateTime SubmittedAt { get; set; }

        public decimal? Score { get; set; }

        public string? Feedback { get; set; }

        public Assignment Assignment { get; set; } = null!;

        public Student Student { get; set; } = null!;
    }
}