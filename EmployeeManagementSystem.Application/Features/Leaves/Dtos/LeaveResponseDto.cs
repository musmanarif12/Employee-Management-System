namespace EmployeeManagementSystem.Application.Features.Leaves.Dtos;

public record LeaveResponseDto(
    int LeaveId,
    DateTime LeaveDate,
    string Reason,
    string Status,
    string? ManagerComment,
    string NotificationMessage
);