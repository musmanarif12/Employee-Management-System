using EmployeeManagementSystem.Application.Features.Leaves.Dtos;
using MediatR;

namespace EmployeeManagementSystem.Application.Features.Leaves.Queries.GetManagerLeaves
{
    public record GetManagerLeavesQuery(int ManagerId) : IRequest<List<LeaveResponseDto>>;
}
