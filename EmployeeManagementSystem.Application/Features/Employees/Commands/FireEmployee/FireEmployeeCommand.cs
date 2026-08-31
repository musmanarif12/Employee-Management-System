using System.Threading;
using System.Threading.Tasks;
using EmployeeManagementSystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Application.Features.Employees.Commands.FireEmployee;

public record FireEmployeeCommand(int TargetEmployeeId, int LoggedInUserId, string CeoRoleName) : IRequest<string>;

public class FireEmployeeCommandHandler : IRequestHandler<FireEmployeeCommand, string>
{
    private readonly IAppDbContext _context;

    public FireEmployeeCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(FireEmployeeCommand request, CancellationToken cancellationToken)
    {
        if (request.CeoRoleName != "CEO")
        {
            return "Unauthorized Access: Only the CEO has authority to fire employees.";
        }

        if (request.TargetEmployeeId == request.LoggedInUserId)
        {
            return "Action Forbidden: You cannot fire yourself!";
        }

        var employee = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.TargetEmployeeId, cancellationToken);

        if (employee == null)
        {
            return "Error: Target employee not found.";
        }

        if (employee.RoleId == 1)
        {
            return $"Action Forbidden: Employee '{employee.FullName}' is a CEO/Administrator and cannot be fired.";
        }

        if (!employee.IsActive)
        {
            return $"Employee '{employee.FullName}' is already marked as fired/inactive.";
        }

        employee.IsActive = false;
        employee.UpdatedAt = System.DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return $"Employee '{employee.FullName}' (ID: {employee.Id}) has been successfully fired.";
    }
}