using MediatR;

namespace EmployeeManagementSystem.Application.Features.Leaves.Commands.ApplyLeave;

public record ApplyLeaveCommand(
    int EmployeeId,
    DateTime LeaveDate,
    string Reason
) : IRequest<string>;