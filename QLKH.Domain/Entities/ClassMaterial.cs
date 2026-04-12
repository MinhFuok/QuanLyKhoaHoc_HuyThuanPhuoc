using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Threading.Tasks;

namespace QLKH.Domain.Entities
{
    public class ClassMaterial
    {
        public int Id { get; set; }

        public int ClassRoomId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string FilePath { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public ClassRoom ClassRoom { get; set; } = null!;
    }
}