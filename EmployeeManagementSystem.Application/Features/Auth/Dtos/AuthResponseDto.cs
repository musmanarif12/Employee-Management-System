namespace EmployeeManagementSystem.Application.Features.Auth.Dtos
{
    public record AuthResponseDto(int Id, string FullName, string Email, string Role, string Token);

}
