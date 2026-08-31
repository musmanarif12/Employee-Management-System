using EmployeeManagementSystem.Application.Common.Interfaces;
using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Application.Features.Leaves.Commands.ApplyLeave;

public class ApplyLeaveCommandHandler : IRequestHandler<ApplyLeaveCommand, string>
{
    private readonly IAppDbContext _context;
    private const int MaxLeaveQuota = 5; 
    public ApplyLeaveCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(ApplyLeaveCommand request, CancellationToken cancellationToken)
    {
        var employee = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.EmployeeId, cancellationToken);

        if (employee == null)
        {
            throw new Exception("Employee not found.");
        }

        if (employee.ReportToId == null && employee.RoleId != 1)
        {
            return "Cannot apply leave: You do not have an assigned Manager/Supervisor to report to.";
        }

        int usedLeaveCount = await _context.LeaveRequests
            .CountAsync(l => l.EmployeeId == request.EmployeeId &&
                             (l.Status == LeaveStatus.Approved || l.Status == LeaveStatus.Pending),
                        cancellationToken);

        if (usedLeaveCount >= MaxLeaveQuota)
        {
            return $"Leave Application Declined: You have reached your maximum limit of {MaxLeaveQuota} leaves. Please contact HR or your manager for further assistance.";
        }

        var leave = new LeaveRequest
        {
            EmployeeId = request.EmployeeId,
            LeaveDate = request.LeaveDate,
            Reason = request.Reason,
            Status = LeaveStatus.Pending
        };

        _context.LeaveRequests.Add(leave);
        await _context.SaveChangesAsync(cancellationToken);

        int remainingQuota = MaxLeaveQuota - (usedLeaveCount + 1);
        return $"Leave request submitted successfully. Remaining quota: {remainingQuota}/{MaxLeaveQuota}. Waiting for Manager approval.";
    }
}