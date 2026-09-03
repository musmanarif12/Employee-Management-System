using EmployeeManagementSystem.Application.Features.Employees.Commands.FireEmployee;
using EmployeeManagementSystem.Application.Features.Employees.Commands.UpdateProfile;
using EmployeeManagementSystem.Application.Features.Employees.Queries.GetFiredEmployees;
using EmployeeManagementSystem.Application.Features.Employees.Queries.GetMyProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] 
public class EmployeesController : ControllerBase
{
    private readonly ISender _mediator;

    public EmployeesController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("fire/{employeeId}")]
    public async Task<ActionResult<string>> FireEmployee(int employeeId)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userRole) || userRole != "CEO")
        {
            return StatusCode(403, "Access Denied: Only CEO can perform this action.");
        }

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int loggedInUserId))
        {
            return Unauthorized("Invalid or missing User Claim in JWT Token.");
        }

        var result = await _mediator.Send(new FireEmployeeCommand(employeeId, loggedInUserId, userRole));
        return Ok(result);
    }

    [HttpGet("fired-list")]
    public async Task<ActionResult<List<FiredEmployeeDto>>> GetFiredEmployees()
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userRole) || userRole != "CEO")
        {
            return StatusCode(403, "Access Denied: Only CEO can view fired employees.");
        }

        var result = await _mediator.Send(new GetFiredEmployeesQuery(userRole));
        return Ok(result);
    }
    [HttpPut("me/profile")]
    public async Task<ActionResult<string>> UpdateMyProfile([FromBody] UpdateSelfProfileRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int loggedInUserId))
        {
            return Unauthorized("Invalid or missing User Claim in JWT Token.");
        }

        var command = new UpdateSelfProfileCommand(loggedInUserId, request.Phone, request.Address);
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpPut("hr/update-employee")]
    public async Task<ActionResult<string>> UpdateEmployeeByHr([FromBody] UpdateEmployeeByHrRequest request)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userRole) || (userRole != "HR" && userRole != "CEO"))
        {
            return StatusCode(403, "Access Denied: Only HR or CEO can perform this update.");
        }

        var command = new UpdateEmployeeByHrCommand(
            request.TargetEmployeeId,
            userRole,
            request.Salary,
            request.Designation,
            request.Department,
            request.Phone,
            request.Address
        );

        var result = await _mediator.Send(command);

        return Ok(result);
    }
    [HttpGet("me/profile")]
    [Authorize]
    public async Task<IActionResult> GetMyProfile()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst("sub")?.Value;

        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var result = await _mediator.Send(new GetMyProfileQuery(userId));
        return Ok(result);
    }
}