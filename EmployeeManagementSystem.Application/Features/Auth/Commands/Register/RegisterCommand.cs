using EmployeeManagementSystem.Application.Features.Auth.Dtos;
using MediatR;
namespace EmployeeManagementSystem.Application.Features.Auth.Commands.Register
{
    public record RegisterCommand(string FullName,string Email,string Password,int RoleId,int? ReportToId) : 
        IRequest<AuthResponseDto>;
}
