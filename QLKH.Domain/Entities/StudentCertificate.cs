using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace QLKH.Domain.Entities
{
    public class StudentCertificate
    {
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Tên chứng chỉ là bắt buộc")]
        [StringLength(200)]
        public string CertificateName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? CertificateCode { get; set; }

        [DataType(DataType.Date)]
        public DateTime? IssuedDate { get; set; }

        [StringLength(200)]
        public string? IssuedBy { get; set; }

        [StringLength(500)]
        public string? Note { get; set; }

        [StringLength(500)]
        public string? EvidenceFilePath { get; set; }

        public bool IsApproved { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ExpiryDate { get; set; }
        public Student? Student { get; set; }
    }
}
