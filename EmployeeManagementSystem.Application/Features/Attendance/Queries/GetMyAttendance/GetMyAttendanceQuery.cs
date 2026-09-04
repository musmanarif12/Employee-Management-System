using EmployeeManagementSystem.Application.Common.Interfaces;
using EmployeeManagementSystem.Application.Features.Attendance.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EmployeeManagementSystem.Application.Features.Attendance.Queries.GetMyAttendance;

public record GetMyAttendanceQuery(int UserId) : IRequest<List<MyAttendanceDto>>;

public class GetMyAttendanceQueryHandler : IRequestHandler<GetMyAttendanceQuery, List<MyAttendanceDto>>
{
    private readonly IAppDbContext _context;

    public GetMyAttendanceQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MyAttendanceDto>> Handle(GetMyAttendanceQuery request, CancellationToken cancellationToken)
    {
        var records = await _context.AttendanceRecords
            .AsNoTracking()
            .Where(a => a.UserId == request.UserId)
            .Select(a => new MyAttendanceDto
            {
                Id = a.Id,
                Date = DateTime.Parse(a.Date), 
                CheckInTime = a.CheckInTime,
                CheckOutTime = a.CheckOutTime,
                Status = string.IsNullOrEmpty(a.Status)
                    ? (a.CheckInTime != DateTime.MinValue ? "Present" : "Absent")
                    : a.Status,
                RequestedCheckIn = a.RequestedCheckIn,
                RequestedCheckOut = a.RequestedCheckOut,
                CorrectionReason = a.CorrectionReason
            })
            .ToListAsync(cancellationToken);

        return records;
    }
}