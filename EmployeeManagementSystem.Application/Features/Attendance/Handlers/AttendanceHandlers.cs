using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmployeeManagementSystem.Application.Common.Interfaces;
using EmployeeManagementSystem.Application.Features.Attendance.Commands;
using EmployeeManagementSystem.Application.Features.Attendance.Queries;
using EmployeeManagementSystem.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Application.Features.Attendance.Handlers;

public class AddAttendanceCommandHandler : IRequestHandler<AddAttendanceCommand, string>
{
    private readonly IAppDbContext _context;

    public AddAttendanceCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(AddAttendanceCommand request, CancellationToken cancellationToken)
    {
        if (request.CheckOutTime <= request.CheckInTime)
        {
            return "Error: Check-out time must be greater than Check-in time.";
        }

        string todayDate = request.CheckInTime.ToString("yyyy-MM-dd");

        var existingRecord = await _context.AttendanceRecords
            .FirstOrDefaultAsync(a => a.UserId == request.LoggedInUserId && a.Date == todayDate, cancellationToken);

        if (existingRecord != null)
        {
            return "Restriction Error: Attendance for today has already been logged. You cannot modify it. Please contact HR for adjustments.";
        }

        TimeSpan timeDifference = request.CheckOutTime - request.CheckInTime;
        decimal calculatedHours = Math.Round((decimal)timeDifference.TotalHours, 2);

        var attendance = new AttendanceRecord
        {
            UserId = request.LoggedInUserId,
            CheckInTime = request.CheckInTime,
            CheckOutTime = request.CheckOutTime,
            TotalHours = calculatedHours,
            Date = todayDate
        };

        _context.AttendanceRecords.Add(attendance);
        await _context.SaveChangesAsync(cancellationToken);

        return $"Attendance successfully recorded for {todayDate}. Total Working Hours: {calculatedHours} hrs.";
    }
}

public class UpdateAttendanceByHrCommandHandler : IRequestHandler<UpdateAttendanceByHrCommand, string>
{
    private readonly IAppDbContext _context;

    public UpdateAttendanceByHrCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(UpdateAttendanceByHrCommand request, CancellationToken cancellationToken)
    {
        if (request.LoggedInUserRole != "HR" && request.LoggedInUserRole != "CEO")
        {
            return "Unauthorized Access: Only HR or CEO can modify attendance logs.";
        }

        if (request.CheckOutTime <= request.CheckInTime)
        {
            return "Error: Check-out time must be greater than Check-in time.";
        }

        var record = await _context.AttendanceRecords
            .FirstOrDefaultAsync(a => a.Id == request.AttendanceId, cancellationToken);

        if (record == null)
        {
            return "Error: Attendance record not found.";
        }

        TimeSpan timeDifference = request.CheckOutTime - request.CheckInTime;
        decimal calculatedHours = Math.Round((decimal)timeDifference.TotalHours, 2);

        record.CheckInTime = request.CheckInTime;
        record.CheckOutTime = request.CheckOutTime;
        record.TotalHours = calculatedHours;
        record.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return $"Attendance ID {record.Id} updated by HR. New Total Hours: {calculatedHours} hrs.";
    }
}

public class GetAllAttendanceForHrQueryHandler : IRequestHandler<GetAllAttendanceForHrQuery, List<AttendanceResponseDto>>
{
    private readonly IAppDbContext _context;

    public GetAllAttendanceForHrQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AttendanceResponseDto>> Handle(GetAllAttendanceForHrQuery request, CancellationToken cancellationToken)
    {
        if (request.LoggedInUserRole != "HR" && request.LoggedInUserRole != "CEO")
        {
            throw new UnauthorizedAccessException("Only HR or CEO can view all attendance records.");
        }

        return await _context.AttendanceRecords
            .Include(a => a.User)
            .OrderByDescending(a => a.CheckInTime)
            .Select(a => new AttendanceResponseDto(
                a.Id,
                a.UserId,
                a.User != null ? a.User.FullName : "Unknown",
                a.Date,
                a.CheckInTime,
                a.CheckOutTime,
                a.TotalHours
            ))
            .ToListAsync(cancellationToken);
    }
}