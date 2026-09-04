using MediatR;
using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem.Application.Common.Interfaces;
using EmployeeManagementSystem.Application.Features.Attendance.Commands;

namespace EmployeeManagementSystem.Application.Features.Attendance.Handlers
{
    public class RequestCorrectionCommandHandler : IRequestHandler<RequestCorrectionCommand, bool>
    {
        private readonly IAppDbContext _context;

        public RequestCorrectionCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(RequestCorrectionCommand request, CancellationToken cancellationToken)
        {
            var attendance = await _context.AttendanceRecords
                .FirstOrDefaultAsync(a => a.Id == request.AttendanceId && a.UserId == request.EmployeeId, cancellationToken);

            if (attendance == null)
            {
                return false;
            }

            attendance.RequestedCheckIn = request.RequestedCheckIn;
            attendance.RequestedCheckOut = request.RequestedCheckOut;
            attendance.CorrectionReason = request.Reason;
            attendance.Status = "Correction Requested";

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}