using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmployeeManagementSystem.Application.Common.Interfaces;
using EmployeeManagementSystem.Application.Features.Leaves.Dtos;
using EmployeeManagementSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Application.Features.Leaves.Queries.GetManagerLeaves;

public class GetManagerLeavesQueryHandler : IRequestHandler<GetManagerLeavesQuery, List<LeaveResponseDto>>
{
    private readonly IAppDbContext _context;
    private const int MaxLeaveQuota = 5;

    public GetManagerLeavesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<LeaveResponseDto>> Handle(GetManagerLeavesQuery request, CancellationToken cancellationToken)
    {
        var leaves = await _context.LeaveRequests
            .Include(l => l.Employee)
            .Where(l => l.Employee != null && l.Employee.ReportToId == request.ManagerId)
            .OrderByDescending(l => l.AppliedOn)
            .ToListAsync(cancellationToken);

        var employeeIds = leaves.Select(l => l.EmployeeId).Distinct().ToList();

        var activeLeaves = await _context.LeaveRequests
            .Where(l => employeeIds.Contains(l.EmployeeId) &&
                        (l.Status == LeaveStatus.Approved || l.Status == LeaveStatus.Pending))
            .ToListAsync(cancellationToken);

        var activeLeaveCounts = activeLeaves
            .GroupBy(l => l.EmployeeId)
            .ToDictionary(g => g.Key, g => g.Count());

        var result = new List<LeaveResponseDto>();

        foreach (var l in leaves)
        {
            int usedQuota = activeLeaveCounts.ContainsKey(l.EmployeeId) ? activeLeaveCounts[l.EmployeeId] : 0;
            int remainingQuota = MaxLeaveQuota - usedQuota;
            if (remainingQuota < 0) remainingQuota = 0;

            string employeeInfo = l.Employee != null
                ? $"Submitted by: {l.Employee.FullName} ({l.Employee.Email})"
                : "Submitted by Employee";

            result.Add(new LeaveResponseDto(
                l.Id,
                l.LeaveDate,
                l.Reason,
                l.Status.ToString(),
                l.ManagerComment,
                employeeInfo,
                MaxLeaveQuota,
                usedQuota,
                remainingQuota
            ));
        }

        return result;
    }
}