using FluentValidation;
using movielogger.api.models.requests.users;

namespace movielogger.api.validation.validators;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required")
            .MinimumLength(6).WithMessage("New password must be at least 6 characters long")
            .NotEqual(x => x.CurrentPassword).WithMessage("New password must be different from current password");
    }
} 