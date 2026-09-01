namespace EmployeeManagementSystem.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        // Foreign Key for Role
        public int RoleId { get; set; }
        public Role? Role { get; set; }

        // Self-referencing FK for Manager
        public int? ReportToId { get; set; }
        public User? ReportTo { get; set; }

        public bool IsActive { get; set; } = true;

        // Add Navigation Property for EmployeeProfile
        public EmployeeProfile? Profile { get; set; }

        public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    }
}
