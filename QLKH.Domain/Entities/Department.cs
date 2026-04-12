using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKH.Domain.Entities
{
    public class Department
    {
        public int Id { get; set; }

        public string DepartmentCode { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        public int? ParentDepartmentId { get; set; }
        public Department? ParentDepartment { get; set; }

        public ICollection<Department> Children { get; set; } = new List<Department>();
    }
}