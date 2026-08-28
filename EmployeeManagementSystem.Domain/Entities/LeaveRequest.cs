using EmployeeManagementSystem.Domain.Enums;

namespace EmployeeManagementSystem.Domain.Entities;

public class LeaveRequest
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public User Employee { get; set; } = null!;

    public DateTime LeaveDate { get; set; }
    public string Reason { get; set; } = string.Empty;

    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
    public string? ManagerComment { get; set; }
    public DateTime AppliedOn { get; set; } = DateTime.UtcNow;
}