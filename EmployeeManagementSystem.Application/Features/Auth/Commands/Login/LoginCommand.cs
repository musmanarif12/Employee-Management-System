using EmployeeManagementSystem.Application.Features.Auth.Dtos;
using MediatR;

namespace EmployeeManagementSystem.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<AuthResponseDto>;