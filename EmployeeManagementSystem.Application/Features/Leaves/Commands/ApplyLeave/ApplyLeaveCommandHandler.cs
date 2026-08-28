using EmployeeManagementSystem.Application.Common.Interfaces;
using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Application.Features.Leaves.Commands.ApplyLeave;

public class ApplyLeaveCommandHandler : IRequestHandler<ApplyLeaveCommand, string>
{
    private readonly IAppDbContext _context;

    public ApplyLeaveCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(ApplyLeaveCommand request, CancellationToken cancellationToken)
    {
        var employee = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.EmployeeId, cancellationToken);

        if (employee == null)
            throw new Exception("Employee not found.");

        var leave = new LeaveRequest
        {
            EmployeeId = request.EmployeeId,
            LeaveDate = request.LeaveDate,
            Reason = request.Reason,
            Status = LeaveStatus.Pending
        };

        _context.LeaveRequests.Add(leave);
        await _context.SaveChangesAsync(cancellationToken);

        return "Leave request submitted successfully. Waiting for Manager approval.";
    }
}