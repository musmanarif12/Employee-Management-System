using EmployeeManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace EmployeeManagementSystem.Application.Common.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Role> Roles { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
