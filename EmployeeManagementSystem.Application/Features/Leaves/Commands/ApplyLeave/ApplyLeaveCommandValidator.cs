using FluentValidation;

namespace EmployeeManagementSystem.Application.Features.Leaves.Commands.ApplyLeave;

public class ApplyLeaveCommandValidator : AbstractValidator<ApplyLeaveCommand>
{
    public ApplyLeaveCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .GreaterThan(0).WithMessage("Valid EmployeeId is required.");

        RuleFor(x => x.LeaveDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Leave date cannot be in the past.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required.")
            .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters.");
    }
}