using System;
using System.Collections.Generic;
using MediatR;

namespace EmployeeManagementSystem.Application.Features.Attendance.Queries;

public record AttendanceResponseDto(
    int Id,
    int UserId,
    string EmployeeName,
    string Date,
    DateTime CheckInTime,
    DateTime CheckOutTime,
    decimal TotalHours,
    string Status,
    DateTime? RequestedCheckIn,
    DateTime? RequestedCheckOut,
    string? CorrectionReason
);

public record GetAllAttendanceForHrQuery(
    string LoggedInUserRole
) : IRequest<List<AttendanceResponseDto>>;