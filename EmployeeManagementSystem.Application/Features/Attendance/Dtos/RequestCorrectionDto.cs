namespace EmployeeManagementSystem.Application.Features.Attendance.Dtos
{
    public class RequestCorrectionDto
    {
        public int AttendanceId { get; set; }
        public DateTime RequestedCheckIn { get; set; }
        public DateTime RequestedCheckOut { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}