using EmployeeManagementSystem.Application.Features.Attendance.Commands;
using EmployeeManagementSystem.Application.Features.Attendance.Queries;
using EmployeeManagementSystem.Application.Features.Attendance.Queries.GetMyAttendance;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly IMediator _mediator;

    public AttendanceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("log")]
    public async Task<ActionResult<string>> LogAttendance([FromBody] AddAttendanceRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int loggedInUserId))
        {
            return Unauthorized("Invalid or missing User Claim in Token.");
        }

        var command = new AddAttendanceCommand(loggedInUserId, request.CheckInTime, request.CheckOutTime);
        var result = await _mediator.Send(command);

        if (result.StartsWith("Restriction") || result.StartsWith("Error"))
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("hr/all-records")]
    public async Task<ActionResult<List<AttendanceResponseDto>>> GetAllAttendanceForHr()
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userRole) || (userRole != "HR" && userRole != "CEO"))
        {
            return StatusCode(403, "Access Denied: Only HR or CEO can view attendance records.");
        }

        var query = new GetAllAttendanceForHrQuery(userRole);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpPut("hr/update")]
    public async Task<ActionResult<string>> UpdateAttendanceByHr([FromBody] UpdateAttendanceByHrRequest request)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userRole) || (userRole != "HR" && userRole != "CEO"))
        {
            return StatusCode(403, "Access Denied: Only HR or CEO can update attendance records.");
        }

        var command = new UpdateAttendanceByHrCommand(request.AttendanceId, userRole, request.CheckInTime, request.CheckOutTime);
        var result = await _mediator.Send(command);

        return Ok(result);
    }
    [HttpGet("my-history")]
    [Authorize]
    public async Task<IActionResult> GetMyAttendanceHistory()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst("sub")?.Value;

        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var result = await _mediator.Send(new GetMyAttendanceQuery(userId));
        return Ok(result);
    }
}