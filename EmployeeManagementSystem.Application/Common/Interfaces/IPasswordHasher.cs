namespace EmployeeManagementSystem.Application.Common.Interfaces
{
    public interface IPasswordHasher
    {
        string HashPassword(string Password);
        bool VerifyPassword(string Password, string PasswordHash);
    }
}
