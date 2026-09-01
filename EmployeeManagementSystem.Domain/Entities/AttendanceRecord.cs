namespace EmployeeManagementSystem.Domain.Entities;

public class AttendanceRecord : BaseEntity
{
    public int UserId { get; set; }
    public User? User { get; set; }

    public DateTime CheckInTime { get; set; }
    public DateTime CheckOutTime { get; set; }

    public decimal TotalHours { get; set; }
    public string Date { get; set; } = string.Empty;
}