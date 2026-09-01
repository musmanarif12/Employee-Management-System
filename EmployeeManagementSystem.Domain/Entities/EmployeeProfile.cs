using EmployeeManagementSystem.Domain.Entities;
namespace EmployeeManagementSystem.Domain.Entities
{
    public class EmployeeProfile : BaseEntity
    {
        public int UserId { get; set; }
        public User? User { get; set; }

        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public decimal Salary { get; set; } = 0;
        public string Designation { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;

    }
}
