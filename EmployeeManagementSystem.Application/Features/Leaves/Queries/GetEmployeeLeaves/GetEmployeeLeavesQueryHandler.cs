using EmployeeManagementSystem.Application.Common.Interfaces;
using EmployeeManagementSystem.Application.Features.Leaves.Dtos;
using EmployeeManagementSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Application.Features.Leaves.Queries.GetEmployeeLeaves;

public class GetEmployeeLeavesQueryHandler : IRequestHandler<GetEmployeeLeavesQuery, List<LeaveResponseDto>>
{
    private readonly IAppDbContext _context;

    public GetEmployeeLeavesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<LeaveResponseDto>> Handle(GetEmployeeLeavesQuery request, CancellationToken cancellationToken)
    {
        var leaves = await _context.LeaveRequests
            .Where(l => l.EmployeeId == request.EmployeeId)
            .OrderByDescending(l => l.AppliedOn)
            .ToListAsync(cancellationToken);

        return leaves.Select(l => new LeaveResponseDto(
            l.Id,
            l.LeaveDate,
            l.Reason,
            l.Status.ToString(),
            l.ManagerComment,
            GetNotificationMessage(l.Status, l.ManagerComment)
        )).ToList();
    }

    private string GetNotificationMessage(LeaveStatus status, string? comment)
    {
        return status switch
        {
            LeaveStatus.Approved => $"Your leave request has been APPROVED by the Project Manager. Comment: {comment ?? "None"}",
            LeaveStatus.Rejected => $"Your leave request has been REJECTED by the Project Manager. Comment: {comment ?? "None"}",
            _ => "Your leave request is currently PENDING review by the Project Manager."
        };
    }
}