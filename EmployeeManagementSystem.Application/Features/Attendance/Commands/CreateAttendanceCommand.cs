using System;
using MediatR;

namespace EmployeeManagementSystem.Application.Features.Attendance.Commands;

public record AddAttendanceRequest(
    DateTime CheckInTime,
    DateTime CheckOutTime
);

public record AddAttendanceCommand(
    int LoggedInUserId,
    DateTime CheckInTime,
    DateTime CheckOutTime
) : IRequest<string>;


public record UpdateAttendanceByHrRequest(
    int AttendanceId,
    DateTime CheckInTime,
    DateTime CheckOutTime
);

public record UpdateAttendanceByHrCommand(
    int AttendanceId,
    string LoggedInUserRole,
    DateTime CheckInTime,
    DateTime CheckOutTime
) : IRequest<string>;