using EmployeeManagementSystem.Application.Features.Leaves.Commands.ApplyLeave;
using EmployeeManagementSystem.Application.Features.Leaves.Commands.ReviewLeave;
using EmployeeManagementSystem.Application.Features.Leaves.Dtos;
using EmployeeManagementSystem.Application.Features.Leaves.Queries.GetEmployeeLeaves;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeavesController : ControllerBase
{
    private readonly ISender _mediator;

    public LeavesController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("apply")]
    public async Task<ActionResult<string>> ApplyLeave([FromBody] ApplyLeaveCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("review")]
    public async Task<ActionResult<string>> ReviewLeave([FromBody] ReviewLeaveCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("my-leaves/{employeeId}")]
    public async Task<ActionResult<List<LeaveResponseDto>>> GetMyLeaves(int employeeId)
    {
        var result = await _mediator.Send(new GetEmployeeLeavesQuery(employeeId));
        return Ok(result);
    }
}