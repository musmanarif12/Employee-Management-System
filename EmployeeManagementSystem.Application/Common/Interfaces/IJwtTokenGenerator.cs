using EmployeeManagementSystem.Domain.Entities;
namespace EmployeeManagementSystem.Application.Common.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user, string roleName);
    }
    
}
