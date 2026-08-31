using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmployeeManagementSystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Application.Features.Employees.Queries.GetFiredEmployees;


public record FiredEmployeeDto(
    int Id,
    string FullName,
    string Email,
    int RoleId,
    bool IsActive
);

public record GetFiredEmployeesQuery(string UserRoleName) : IRequest<List<FiredEmployeeDto>>;

public class GetFiredEmployeesQueryHandler : IRequestHandler<GetFiredEmployeesQuery, List<FiredEmployeeDto>>
{
    private readonly IAppDbContext _context;

    public GetFiredEmployeesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<FiredEmployeeDto>> Handle(GetFiredEmployeesQuery request, CancellationToken cancellationToken)
    {
        if (request.UserRoleName != "CEO")
        {
            return new List<FiredEmployeeDto>();
        }

        var firedEmployees = await _context.Users
            .Where(u => !u.IsActive)
            .Select(u => new FiredEmployeeDto(
                u.Id,
                u.FullName,
                u.Email,
                u.RoleId,
                u.IsActive
            ))
            .ToListAsync(cancellationToken);

        return firedEmployees;
    }
}