using FluentValidation;
using movielogger.api.models;
using movielogger.api.models.requests.users;

namespace movielogger.api.validation.validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email Is Required")
            .EmailAddress().WithMessage("Email Is Not Valid");
        RuleFor(x => x.UserName).NotEmpty().WithMessage("Name Is Required");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password Is Required");
    }
}