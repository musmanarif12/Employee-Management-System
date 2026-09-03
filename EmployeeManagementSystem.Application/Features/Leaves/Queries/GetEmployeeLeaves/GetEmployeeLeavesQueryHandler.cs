using EmployeeManagementSystem.Application.Common.Interfaces;
using EmployeeManagementSystem.Application.Features.Leaves.Dtos;
using EmployeeManagementSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EmployeeManagementSystem.Application.Features.Leaves.Queries.GetEmployeeLeaves;

public class GetEmployeeLeavesQueryHandler : IRequestHandler<GetEmployeeLeavesQuery, List<LeaveResponseDto>>
{
    private readonly IAppDbContext _context;
    private const int MaxLeaveQuota = 5;

    public GetEmployeeLeavesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<LeaveResponseDto>> Handle(GetEmployeeLeavesQuery request, CancellationToken cancellationToken)
    {
        var leaves = await _context.LeaveRequests
            .AsNoTracking()
            .Where(l => l.EmployeeId == request.EmployeeId)
            .OrderByDescending(l => l.AppliedOn)
            .ToListAsync(cancellationToken);

        int usedQuota = leaves.Count(l => l.Status == LeaveStatus.Approved || l.Status == LeaveStatus.Pending);
        int remainingQuota = Math.Max(0, MaxLeaveQuota - usedQuota);

        return leaves.Select(l => new LeaveResponseDto(
            l.Id,
            l.LeaveDate,
            l.Reason,
            l.Status.ToString(),
            l.ManagerComment,
            GetNotificationMessage(l.Status, l.ManagerComment),
            MaxLeaveQuota,
            usedQuota,
            remainingQuota
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