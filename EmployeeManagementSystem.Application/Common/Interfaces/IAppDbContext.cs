using EmployeeManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<LeaveRequest> LeaveRequests { get; }
    DbSet<EmployeeProfile> EmployeeProfiles { get; }
    DbSet<AttendanceRecord> AttendanceRecords { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}