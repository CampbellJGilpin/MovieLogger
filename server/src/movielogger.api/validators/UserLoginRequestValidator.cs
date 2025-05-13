using FluentValidation;
using movielogger.api.models;

namespace movielogger.api.validators;

public class UserLoginRequestValidator : AbstractValidator<UserLoginRequest>
{
    public UserLoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email Is Required").EmailAddress();
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
    }
}