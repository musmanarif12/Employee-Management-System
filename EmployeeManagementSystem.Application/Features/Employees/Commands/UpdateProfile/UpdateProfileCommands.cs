using MediatR;

namespace EmployeeManagementSystem.Application.Features.Employees.Commands.UpdateProfile;

public record UpdateSelfProfileRequest(
    string Phone,
    string Address
);

public record UpdateSelfProfileCommand(
    int LoggedInUserId,
    string Phone,
    string Address
) : IRequest<string>;


public record UpdateEmployeeByHrRequest(
    int TargetEmployeeId,
    decimal Salary,
    string Designation,
    string Department,
    string Phone,
    string Address
);

public record UpdateEmployeeByHrCommand(
    int TargetEmployeeId,
    string LoggedInUserRole,
    decimal Salary,
    string Designation,
    string Department,
    string Phone,
    string Address
) : IRequest<string>;