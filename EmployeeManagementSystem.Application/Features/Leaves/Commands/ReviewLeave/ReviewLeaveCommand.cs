using MediatR;

namespace EmployeeManagementSystem.Application.Features.Leaves.Commands.ReviewLeave;

public record ReviewLeaveCommand(
    int LeaveId,
    bool IsApproved,
    string? ManagerComment
) : IRequest<string>;