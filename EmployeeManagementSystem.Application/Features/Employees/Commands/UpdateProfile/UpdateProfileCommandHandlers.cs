using System;
using System.Threading;
using System.Threading.Tasks;
using EmployeeManagementSystem.Application.Common.Interfaces;
using EmployeeManagementSystem.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Application.Features.Employees.Commands.UpdateProfile;

public class UpdateSelfProfileCommandHandler : IRequestHandler<UpdateSelfProfileCommand, string>
{
    private readonly IAppDbContext _context;

    public UpdateSelfProfileCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(UpdateSelfProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == request.LoggedInUserId, cancellationToken);

        if (user == null || !user.IsActive)
        {
            return "Error: User account not found or inactive.";
        }

        if (user.Profile == null)
        {
            user.Profile = new EmployeeProfile
            {
                UserId = user.Id,
                Phone = request.Phone,
                Address = request.Address
            };
            _context.EmployeeProfiles.Add(user.Profile);
        }
        else
        {
            user.Profile.Phone = request.Phone;
            user.Profile.Address = request.Address;
            user.Profile.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return "Your profile details have been updated successfully.";
    }
}

public class UpdateEmployeeByHrCommandHandler : IRequestHandler<UpdateEmployeeByHrCommand, string>
{
    private readonly IAppDbContext _context;

    public UpdateEmployeeByHrCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(UpdateEmployeeByHrCommand request, CancellationToken cancellationToken)
    {
        if (request.LoggedInUserRole != "HR" && request.LoggedInUserRole != "CEO")
        {
            return "Unauthorized Access: Only HR or CEO can modify salary and administrative employee details.";
        }

        var targetUser = await _context.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == request.TargetEmployeeId, cancellationToken);

        if (targetUser == null)
        {
            return "Error: Target employee not found.";
        }

        if (targetUser.Profile == null)
        {
            targetUser.Profile = new EmployeeProfile
            {
                UserId = targetUser.Id,
                Salary = request.Salary,
                Designation = request.Designation,
                Department = request.Department,
                Phone = request.Phone,
                Address = request.Address
            };
            _context.EmployeeProfiles.Add(targetUser.Profile);
        }
        else
        {
            targetUser.Profile.Salary = request.Salary;
            targetUser.Profile.Designation = request.Designation;
            targetUser.Profile.Department = request.Department;
            targetUser.Profile.Phone = request.Phone;
            targetUser.Profile.Address = request.Address;
            targetUser.Profile.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return $"Employee '{targetUser.FullName}' profile and salary details successfully updated by HR.";
    }
}