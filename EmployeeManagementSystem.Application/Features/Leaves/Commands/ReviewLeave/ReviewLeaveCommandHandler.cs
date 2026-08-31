using System;
using System.Threading;
using System.Threading.Tasks;
using EmployeeManagementSystem.Application.Common.Interfaces;
using EmployeeManagementSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Application.Features.Leaves.Commands.ReviewLeave;

public class ReviewLeaveCommandHandler : IRequestHandler<ReviewLeaveCommand, string>
{
    private readonly IAppDbContext _context;

    public ReviewLeaveCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(ReviewLeaveCommand request, CancellationToken cancellationToken)
    {
        var leave = await _context.LeaveRequests
            .Include(l => l.Employee)
            .FirstOrDefaultAsync(l => l.Id == request.LeaveId, cancellationToken);

        if (leave == null)
        {
            return "Error: Leave request not found.";
        }

        if (leave.Employee != null && leave.Employee.ReportToId != request.ManagerId)
        {
            return $"Unauthorized Access: You (Manager ID: {request.ManagerId}) do not have permission to review leave for '{leave.Employee.FullName}'.";
        }

        leave.Status = request.IsApproved ? LeaveStatus.Approved : LeaveStatus.Rejected;
        leave.ManagerComment = request.ManagerComment;

        await _context.SaveChangesAsync(cancellationToken);

        string statusText = request.IsApproved ? "Approved" : "Rejected";
        return $"Leave request #{leave.Id} has been {statusText} successfully.";
    }
}