using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using EmployeeManagementSystem.Application.Features.Leaves.Commands.ApplyLeave;
using EmployeeManagementSystem.Application.Features.Leaves.Commands.ReviewLeave;
using EmployeeManagementSystem.Application.Features.Leaves.Dtos;
using EmployeeManagementSystem.Application.Features.Leaves.Queries.GetEmployeeLeaves;
using EmployeeManagementSystem.Application.Features.Leaves.Queries.GetManagerLeaves;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
    public async Task<ActionResult<string>> ReviewLeave([FromBody] ReviewLeaveRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int loggedInManagerId))
        {
            return Unauthorized("Invalid or missing User Claim in JWT Token.");
        }

        var reviewCommand = new ReviewLeaveCommand(
            request.LeaveId,
            loggedInManagerId,
            request.IsApproved,
            request.ManagerComment
        );

        var result = await _mediator.Send(reviewCommand);
        return Ok(result);
    }

    [HttpGet("my-leaves/{employeeId}")]
    public async Task<ActionResult<List<LeaveResponseDto>>> GetMyLeaves(int employeeId)
    {
        var result = await _mediator.Send(new GetEmployeeLeavesQuery(employeeId));
        return Ok(result);
    }

    [HttpGet("manager-pending-leaves")]
    [Authorize(Roles = "ProjectManager")]
    public async Task<ActionResult<List<LeaveResponseDto>>> GetManagerLeaves()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int loggedInManagerId))
        {
            return Unauthorized("Invalid or missing User Claim in JWT Token.");
        }

        var result = await _mediator.Send(new GetManagerLeavesQuery(loggedInManagerId));
        return Ok(result);
    }
}

public record ReviewLeaveRequest(
    int LeaveId,
    bool IsApproved,
    string? ManagerComment
);