namespace EmployeeManagementSystem.Domain.Entities
{
    public class AttendanceRecord
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        // Types corrected to match Handlers/Queries
        public string Date { get; set; } = string.Empty;
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
        public decimal TotalHours { get; set; }

        // Correction Workflow Fields
        public DateTime? RequestedCheckIn { get; set; }
        public DateTime? RequestedCheckOut { get; set; }
        public string? CorrectionReason { get; set; }
        public string Status { get; set; } = "Present";

        public DateTime? UpdatedAt { get; set; }
    }
}