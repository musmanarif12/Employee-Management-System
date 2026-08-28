using EmployeeManagementSystem.Application.Features.Leaves.Dtos;
using MediatR;

namespace EmployeeManagementSystem.Application.Features.Leaves.Queries.GetEmployeeLeaves;

public record GetEmployeeLeavesQuery(int EmployeeId) : IRequest<List<LeaveResponseDto>>;