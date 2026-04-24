namespace QLKH.Web.Areas.Admin.Models
{
    public class UserListItemViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsLocked { get; set; }

        public bool IsLinkedStudent { get; set; }
        public bool IsLinkedTeacher { get; set; }
        public string LinkedLabel { get; set; } = "Chưa liên kết";
    }
}