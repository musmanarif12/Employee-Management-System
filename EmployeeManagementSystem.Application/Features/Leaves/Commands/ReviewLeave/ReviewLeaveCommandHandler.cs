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
            .FirstOrDefaultAsync(l => l.Id == request.LeaveId, cancellationToken);

        if (leave == null)
            throw new Exception("Leave request not found.");

        leave.Status = request.IsApproved ? LeaveStatus.Approved : LeaveStatus.Rejected;
        leave.ManagerComment = request.ManagerComment;

        await _context.SaveChangesAsync(cancellationToken);

        string statusText = request.IsApproved ? "Approved" : "Rejected";
        return $"Leave request has been {statusText} by Manager.";
    }
}