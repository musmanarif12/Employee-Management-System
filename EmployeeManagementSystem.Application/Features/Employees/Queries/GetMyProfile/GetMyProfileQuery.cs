using EmployeeManagementSystem.Application.Common.Interfaces;
using EmployeeManagementSystem.Application.Features.Employees.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EmployeeManagementSystem.Application.Features.Employees.Queries.GetMyProfile;

public record GetMyProfileQuery(int UserId) : IRequest<EmployeeProfileDto>;

public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, EmployeeProfileDto>
{
    private readonly IAppDbContext _context;

    public GetMyProfileQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<EmployeeProfileDto> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
            throw new Exception("User profile not found.");

        return new EmployeeProfileDto
        {
            Id = user.Id,
            FullName = user.FullName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Role = user.Role?.ToString() ?? "Employee" // Null safe mapping
        };
    }
}