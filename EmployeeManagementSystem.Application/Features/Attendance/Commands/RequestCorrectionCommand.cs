using MediatR;

namespace EmployeeManagementSystem.Application.Features.Attendance.Commands
{
    public class RequestCorrectionCommand : IRequest<bool>
    {
        public int AttendanceId { get; set; }
        public DateTime RequestedCheckIn { get; set; }
        public DateTime RequestedCheckOut { get; set; }
        public string Reason { get; set; } = string.Empty;
        public int EmployeeId { get; set; }
    }
}