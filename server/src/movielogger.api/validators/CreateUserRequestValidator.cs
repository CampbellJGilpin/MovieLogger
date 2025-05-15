using FluentValidation;
using movielogger.api.models;
using movielogger.api.models.requests.users;

namespace movielogger.api.validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
            
    }
}