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
            .Select(a => new
            {
                a.Id,
                a.Date,
                a.CheckInTime,
                a.CheckOutTime
            })
            .ToListAsync(cancellationToken);

        return records.Select(a => new MyAttendanceDto
        {
            Id = a.Id,
            Date = a.Date,
            CheckInTime = a.CheckInTime,
            CheckOutTime = a.CheckOutTime,
            Status = a.CheckInTime != DateTime.MinValue ? "Present" : "Absent"
        }).ToList();
    }
}