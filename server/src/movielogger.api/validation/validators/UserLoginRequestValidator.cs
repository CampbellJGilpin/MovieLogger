using FluentValidation;
using movielogger.api.models;
using movielogger.api.models.requests.users;

namespace movielogger.api.validation.validators;

public class UserLoginRequestValidator : AbstractValidator<LoginUserRequest>
{
    public UserLoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email Is Required")
            .EmailAddress().WithMessage("Email Is Not Valid");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
    }
}