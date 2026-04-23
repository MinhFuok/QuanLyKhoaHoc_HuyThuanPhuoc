namespace QLKH.Web.Areas.Admin.Models
{
    public class DeleteUserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public bool IsLinkedStudent { get; set; }
        public bool IsLinkedTeacher { get; set; }

        public string? LinkedStudentCode { get; set; }
        public string? LinkedTeacherCode { get; set; }

        public bool CanDelete => !IsLinkedStudent && !IsLinkedTeacher;
    }
}