namespace EmployeeManagementSystem.Application.Features.Attendance.Dtos
{
    public class MyAttendanceDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }

        public string Status { get; set; } = string.Empty;
        public DateTime? RequestedCheckIn { get; set; }
        public DateTime? RequestedCheckOut { get; set; }
        public string? CorrectionReason { get; set; }
    }
}