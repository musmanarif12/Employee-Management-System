using System;

namespace EmployeeManagementSystem.Application.Features.Attendance.Dtos;

public class MyAttendanceDto
{
    public int Id { get; set; }
    public string Date { get; set; } = string.Empty;
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string Status { get; set; } = string.Empty;
}