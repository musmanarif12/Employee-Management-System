using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using EmployeeManagementSystem.Application.Features.Employees.Commands.FireEmployee;
using EmployeeManagementSystem.Application.Features.Employees.Queries.GetFiredEmployees;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
}